using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Project.Development.BotScripts;
using Project.Development.CameraScripts;
using Project.Development.CharacterScripts;
using Project.Development.Events;
using Project.Development.InputController;
using Project.Development.ModeScripts;
using Project.Development.Perks;
using Project.Development.SkinScripts;
using Project.Development.UI;
using UnityEngine;
using UnityEngine.Scripting;
using Random = UnityEngine.Random;

namespace Project.Development
{
	public class Spawner: MonoBehaviour
	{
		public static Spawner Instance { get; private set; }
	
		private const string LevelsInfoKey = "levelinfo";
		private const string NextActivePerkLevelKey = "nextperklevel";
	
		private static readonly int Color1F = Shader.PropertyToID("_Color1_F");
		private static readonly int Color1B = Shader.PropertyToID("_Color1_B");
		private static readonly int Color1L = Shader.PropertyToID("_Color1_L");
		private static readonly int Color1R = Shader.PropertyToID("_Color1_R");
		private static readonly int Color1T = Shader.PropertyToID("_Color1_T");
		private static readonly int Color1D = Shader.PropertyToID("_Color1_D");
		private static readonly int ColorFog = Shader.PropertyToID("_Color_Fog");
	
		[SerializeField] private Character characterPrefab;
		[SerializeField] private CameraController cameraControllerPrefab;
		[SerializeField] private GameMode gameModePrefab;
		[SerializeField] private BonusLevelGameMode bonusLevelGameModePrefab;
		[SerializeField] private TextAsset nicknamesFile;
		[SerializeField] private MeshNickname meshNicknamePrefab;
		[SerializeField] private ActivePerkPickup[] activePerkPickupPrefabs;
		[SerializeField] private float moveSpeedOnFirstLevels;
		public int BonusLevelNum;

		public IPlayerProfile PlayerProfile { get; private set; }
		public Dictionary<GameObject, MeshNickname> CharacterMeshNicknames { get; set; }

		private HideAndSeekGameMode hideAndSeekGameMode;
		
		private Character character;
		
		private string[] nicknames;
		public static Vector3 CurrLocationPos;
		private GameObject currLocation;
		private GameObject prevLocation;
		private GameObject currBackground;
		private GameObject prevBackground;

		private LevelInfoWrapper levelInfoWrapper;

		public int NextActivePerkLevel
		{
			get => PlayerPrefs.GetInt(NextActivePerkLevelKey);
			set
			{
				PlayerPrefs.SetInt(NextActivePerkLevelKey, value);
				PlayerPrefs.Save();
			}
		}

		private List<ActivePerkPickup> activePerkPickups;


		private void OnApplicationQuit()
		{
			SaveLevelsInfo();
		}

		private void OnApplicationFocus(bool hasFocus)
		{
			if (!hasFocus)
			{
				SaveLevelsInfo();
			}
		}

		private void OnApplicationPause(bool pauseStatus)
		{
			SaveLevelsInfo();
		}

		private void Awake()
		{
			if (Instance == null)
			{
				Instance = this;
				DontDestroyOnLoad(this);
			}
			else 
			{
				if (Instance != this)
				{
					Destroy(this.gameObject);
                    
					return;
				}
                
				return;
			}

			PlayerProfile = FindObjectOfType<PlayerProfile>();
			
			CharacterMeshNicknames = new Dictionary<GameObject, MeshNickname>();
		
			LoadNicknames();

			LoadLevelsInfo();
			
			this.activePerkPickups = new List<ActivePerkPickup>();
			
			EventManager.OnActivePerkPickedUp.AddListener(OnActivePerkPickedUp);
		}

		private void Start()
		{
			if (!PlayerPrefs.HasKey(NextActivePerkLevelKey))
			{
				RandomNextActivePerkLevel();
			}
		
			var newLevel = LevelRepository.Instance.CurrentLevel;
			
			InitLevelInfo(LevelRepository.Instance.CurrentLevelNumber);
			LoadColorSchemes(newLevel, LevelRepository.Instance.CurrentLevelNumber);

			var rotation = GetLevelRotation(LevelRepository.Instance.CurrentLevel,
				LevelRepository.Instance.CurrentLevelNumber);
		
			this.currLocation = Instantiate(LevelRepository.Instance.CurrentLevel.Location, CurrLocationPos,
				Quaternion.Euler(0, rotation, 0));
				
			ApplyMaterials(newLevel);

			this.currBackground = InstantiateBackground(null, LevelRepository.Instance.CurrentLevel, 1.0f);
			
			InitActivePerks(LevelRepository.Instance.CurrentLevelNumber);
			
			StartCoroutine(InitLevel(LevelRepository.Instance.CurrentLevel, false));
		}

		private void OnDestroy()
		{
			EventManager.OnActivePerkPickedUp.RemoveListener(OnActivePerkPickedUp);
		}

		private float GetLevelRotation(Level level, int levelNumber)
		{
			float rotation;
			
			var levelInfo = this.levelInfoWrapper.LevelsInfo.SingleOrDefault(l =>
				l.LevelNumber == levelNumber);
			
			if (levelInfo == null || levelInfo.Rotation < -180f)
			{
				rotation = level.Rotations[Random.Range(0, level.Rotations.Length)];

				if (levelInfo != null && levelInfo.Rotation < -180f)
				{
					levelInfo.Rotation = rotation;
				}
			}
			else
			{
				rotation = levelInfo.Rotation;
			}

			return rotation;
		}


		public Character SpawnCharacter(Character characterPrefab, Vector3 pos, Quaternion rot)
		{
			var character = Instantiate(characterPrefab, pos, rot);

			character.gameObject.tag = "Player";
			character.SetSkin(FindObjectOfType<PlayerProfile>().SkinService.CurrentSkin);
		
			return character;
		}
		
		public Character SpawnBotCharacter(Character characterPrefab, Vector3 pos, Quaternion rot)
		{
			var character = Instantiate(characterPrefab, pos, rot);

			character.gameObject.AddComponent<BotController>();

			if (LevelRepository.Instance.CurrentLevelNumber <= 5)
			{
				character.MovementController.MoveSpeed = this.moveSpeedOnFirstLevels;
			}
			
			character.InputController = new MockInputController();
			
			character.gameObject.AddComponent<MockPlayerProfile>();

			var movementController = character.GetComponent<MovementController>();

			character.SetSkin(SkinRepository.Instance.Skins[Random.Range(0, SkinRepository.Instance.Skins.Length)]);
			
			movementController.Gravity = Vector3.zero;
			movementController.AlwaysGrounded = true;
		
			return character;
		}

		public CameraController SpawnCameraController(CameraController cameraController, Vector3 pos, Quaternion rot)
		{
			return Instantiate(cameraController, pos, rot);
		}

		public void ChangeLevel(Level newLevel, int levelNum, bool bonus = false)
		{
			EventManager.OnChangeLevel?.Invoke(newLevel, levelNum, bonus);
		
			StartCoroutine(ChangeLevelProcess(newLevel, levelNum, bonus));
		}

		public void RestartLevel()
		{
			EventManager.OnChangeLevel?.Invoke(null, LevelRepository.Instance.CurrentLevelNumber, false);
		
			StartCoroutine(RestartLevelProcess());
		}

		public IEnumerator RestartLevelProcess()
		{
			this.prevLocation = this.currLocation;
		
			yield return StartCoroutine(DestroyObjects());

			var rotation = GetLevelRotation(LevelRepository.Instance.CurrentLevel,
				LevelRepository.Instance.CurrentLevelNumber);

			this.currLocation = Instantiate(LevelRepository.Instance.CurrentLevel.Location, CurrLocationPos, Quaternion.Euler(0, rotation, 0));
			
			yield return null;
			
			ApplyMaterials(LevelRepository.Instance.CurrentLevel);
			
			yield return null;
			
			InitActivePerks(LevelRepository.Instance.CurrentLevelNumber);

			yield return StartCoroutine(InitLevel(LevelRepository.Instance.CurrentLevel, true));
			
			EventManager.OnLevelChanged?.Invoke(LevelRepository.Instance.CurrentLevel, LevelRepository.Instance.CurrentLevelNumber, false);
		}

		private IEnumerator ChangeLevelProcess(Level newLevel, int levelNum, bool bonus)
		{
			if (!bonus)
			{
				InitLevelInfo(levelNum);
			}

			LoadColorSchemes(newLevel, levelNum);

			if (NextActivePerkLevel == LevelRepository.Instance.CurrentLevelNumber)
			{
				RandomNextActivePerkLevel();
			}
		
			CurrLocationPos += Vector3.forward * 45f;
		
			var cameraController = FindObjectOfType<CameraController>();

			cameraController.GetComponent<Animator>().enabled = false;

			yield return null;
			
			var motionTarget = new GameObject();

			motionTarget.transform.position = new Vector3(0, 35f, CurrLocationPos.z - 20f);
			
			cameraController.MotionTarget = motionTarget.transform;
			
			cameraController.LerpCamera(Quaternion.Euler(new Vector3(60f, 0,0)), new Vector3(0, 0f, 0f), bonus ? 0.75f : 1.5f);

			this.prevLocation = this.currLocation;

			var rotation = GetLevelRotation(newLevel, levelNum);
			
			this.currLocation = Instantiate(newLevel.Location, CurrLocationPos, Quaternion.Euler(0, rotation, 0));
			
			yield return null;
			
			ApplyMaterials(newLevel);
			
			yield return null;
			
			this.prevBackground = this.currBackground;

			this.currBackground = InstantiateBackground(LevelRepository.Instance.CurrentLevel, newLevel, bonus ? 0.5f : 1f);
			
			Destroy(this.prevBackground, 3.0f);
			
			yield return new WaitForSeconds(bonus ? 0.4f : 1.0f);
		
			yield return StartCoroutine(DestroyObjects());
			
			InitActivePerks(LevelRepository.Instance.CurrentLevelNumber);
			
			yield return null;

			if (bonus)
			{
				yield return StartCoroutine(InitBonusLevel(newLevel, levelNum));
			}
			else
			{
				yield return StartCoroutine(InitLevel(newLevel, false));
			}

			if (!bonus)
			{
				EventManager.OnLevelChanged?.Invoke(newLevel, levelNum, false);
			}
		}

		private IEnumerator InitLevel(Level level, bool restart)
		{
			var respawns = GameObject.FindGameObjectsWithTag("Respawn");

			yield return null;
			
			this.character = SpawnCharacter(this.characterPrefab, respawns[0].transform.position, respawns[0].transform.rotation);
			
			yield return null;
			
			this.hideAndSeekGameMode = Instantiate(this.gameModePrefab) as HideAndSeekGameMode;

			SpawnBots(this.hideAndSeekGameMode.PlayersCount, respawns);
			
			yield return null;
			
			ActivateRandomDoorPattern();
			
			yield return null;
			
			ActivateRandomPuddlePattern();

			yield return null;

			this.character.InputController = FindObjectOfType<JoystickController>();

			yield return null;

			var cameraController = FindObjectOfType<CameraController>();

			if (cameraController == null)
			{
				cameraController = SpawnCameraController(this.cameraControllerPrefab, respawns[0].transform.position
				                                                                + CameraRepository.Instance.Offset,
					Quaternion.Euler(CameraRepository.Instance.Rotation));
					
				cameraController.enabled = false;
				
				var camera = cameraController.GetComponent<Camera>();

				camera.backgroundColor = level.BackgroundMaterial.GetColor(ColorFog);
			}
			else
			{
				cameraController.enabled = true;
			}
			
			this.character.CameraController = cameraController;
			
			if (restart)
			{
				cameraController.MotionTarget = this.character.transform;
			}
		}

		public void InitActivePerks(int levelNumber)
		{
			if (NextActivePerkLevel != levelNumber)
			{
				return;
			}
		
			var levelInfo = this.levelInfoWrapper.LevelsInfo.SingleOrDefault(l =>
				l.LevelNumber == levelNumber);
				
			if (levelInfo != null)
			{
				if (!string.IsNullOrEmpty(levelInfo.ActivePerkPlace))
				{
					if (Convert.ToBoolean(levelInfo.IsActivePerkAvailable))
					{
						var levelActivePerk =
							(ActivePerkName) Enum.Parse(typeof(ActivePerkName), levelInfo.ActivePerkName);

						var perkPlace = GameObject.Find(levelInfo.ActivePerkPlace);
						var activePerk = this.activePerkPickupPrefabs.First(p => p.ActivePerkName == levelActivePerk);

						var perkPickup = Instantiate(activePerk,
							perkPlace.transform.position, perkPlace.transform.rotation);
							
						perkPickup.gameObject.SetActive(false);

						this.activePerkPickups.Add(perkPickup);
					}
				}
				else
				{
					var perksPlace = GameObject.FindGameObjectsWithTag("PerkPlace");
	
					if (perksPlace.Length > 0)
					{
						var randomPerkPlace = perksPlace[Random.Range(0, perksPlace.Length)];
	
						var randomActivePerk =
							this.activePerkPickupPrefabs[Random.Range(0, this.activePerkPickupPrefabs.Length)];
	
						var perkPickup = Instantiate(randomActivePerk,
							randomPerkPlace.transform.position, randomPerkPlace.transform.rotation);
							
						perkPickup.gameObject.SetActive(false);
						
						this.activePerkPickups.Add(perkPickup);

						levelInfo.ActivePerkPlace = randomPerkPlace.name;
						levelInfo.ActivePerkName = randomActivePerk.ActivePerkName.ToString();
						levelInfo.IsActivePerkAvailable = 1;
						levelInfo.IsActivePerkShown = 0;
					}
				}
				
				if (levelInfo.IsActivePerkShown == 1)
				{
					ShowActivePerks();
				}
			}
		}

		public void ShowActivePerks()
		{
			var levelInfo = this.levelInfoWrapper.LevelsInfo.SingleOrDefault(l =>
				l.LevelNumber == LevelRepository.Instance.CurrentLevelNumber);
			
			if (levelInfo != null && levelInfo.IsActivePerkShown == 0)
			{
				levelInfo.IsActivePerkShown = 1;
			}

			foreach (var perkPickup in this.activePerkPickups)
			{
				if (perkPickup == null)
				{
					continue;
				}
				
				perkPickup.gameObject.SetActive(true);
			}
		}

		private IEnumerator InitBonusLevel(Level level, int levelNum)
		{
			var respawns = GameObject.FindGameObjectsWithTag("Respawn");
			
			this.character = SpawnCharacter(this.characterPrefab, respawns[0].transform.position, respawns[0].transform.rotation);

			this.character.InputController = FindObjectOfType<JoystickController>();

			var cameraController = FindObjectOfType<CameraController>();

			if (cameraController == null)
			{
				cameraController = SpawnCameraController(this.cameraControllerPrefab, respawns[0].transform.position
				                                                                      + CameraRepository.Instance.Offset,
					Quaternion.Euler(CameraRepository.Instance.Rotation));
					
				cameraController.enabled = false;
				
				var camera = cameraController.GetComponent<Camera>();

				camera.backgroundColor = level.BackgroundMaterial.GetColor(ColorFog);
			}
			else
			{
				cameraController.enabled = true;
			}
			
			this.character.CameraController = cameraController;
			
			yield return null;
			
			EventManager.OnLevelChanged?.Invoke(level, levelNum, true);

			var bonusLevelGameMode = Instantiate(this.bonusLevelGameModePrefab);
			
			bonusLevelGameMode.Join(this.character, null);
			
			yield return new WaitForSeconds(0.5f);
			
			bonusLevelGameMode.StartMatch();
			
			
		}

		private IEnumerator DestroyObjects()
		{
			var gameMode = FindObjectOfType<GameMode>();

			if (gameMode != null)
			{
				Destroy(gameMode.gameObject);
			}

			var meshNicknames = FindObjectsOfType<MeshNickname>();

			foreach (var meshNickname in meshNicknames)
			{
				Destroy(meshNickname.gameObject);
			}
			
			CharacterMeshNicknames.Clear();

			if (this.prevLocation != null)
			{
				Destroy(this.prevLocation);
			}

			yield return null;

			var characters = FindObjectsOfType<Character>();

			foreach (var character in characters)
			{
				Destroy(character.gameObject);
			}

			foreach (var perk in this.activePerkPickups)
			{
				if (perk == null)
				{
					continue; 
				}
				
				Destroy(perk.gameObject);
			}

			var cages = FindObjectsOfType<Cage>();

			foreach (var cage in cages)
			{
				Destroy(cage.gameObject);
			}
		}

		private void ActivateRandomDoorPattern()
		{
			var doorPatterns = this.currLocation.transform.Find("DoorPatterns");
			
			if(doorPatterns == null)
			{
				doorPatterns = this.currLocation.transform.Find("Location").Find("DoorPatterns");
			}
			
			if (doorPatterns != null && doorPatterns.childCount > 0)
			{
				foreach (Transform door in doorPatterns)
				{
					door.gameObject.SetActive(false);
				}
			
				doorPatterns.GetChild(Random.Range(0, doorPatterns.childCount)).gameObject.SetActive(true);
			}
		}
		
		private void ActivateRandomPuddlePattern()
		{
			var puddlePatterns = this.currLocation.transform.Find("PuddlePatterns");
			
			if(puddlePatterns == null)
			{
				puddlePatterns = this.currLocation.transform.Find("Location").Find("PuddlePatterns");
			}
			
			if (puddlePatterns != null && puddlePatterns.childCount > 0)
			{
				foreach (Transform puddlePattern in puddlePatterns)
				{
					puddlePattern.gameObject.SetActive(false);
				}

				var randomPuddlePattern = puddlePatterns.GetChild(Random.Range(0, puddlePatterns.childCount));

				foreach (Transform puddle in randomPuddlePattern)
				{
					puddle.transform.localRotation = Quaternion.Euler(new Vector3(-90f, 
						Random.Range(0, 360),puddle.transform.localEulerAngles.z));
					puddle.localScale = new Vector3(65,65,65);

					var randomPuddleFootstep = PuddleRepository.Instance.GetRandomPuddleFootstep();

					var puddleComp = puddle.GetComponent<Puddle>();

					puddleComp.FootprintsMaterial = randomPuddleFootstep.FootstepMaterial;
					puddleComp.Material = randomPuddleFootstep.PuddleMaterial;
				}
			
				randomPuddlePattern.gameObject.SetActive(true);
			}
		}
		
		private void SetRandomProfileInfo(Character[] characters)
		{
			var nicknames = this.nicknames.ToList();

			foreach (var character in characters)
			{
				var randomNickname = nicknames[Random.Range(0, nicknames.Count)];

				var playerProfile = character.GetComponent<IPlayerProfile>();
			
				playerProfile.Name = randomNickname;
				
				var countries = Enum.GetValues(typeof(Country)) as Country[];
				
				playerProfile.Country = countries[Random.Range(0, countries.Length)];

				nicknames.Remove(randomNickname);
				
				var meshNickname = InstantiateMeshNickname(playerProfile.Name, character.transform, new Vector3(0f, 2f, 0f));
				
				CharacterMeshNicknames.Add(character.gameObject, meshNickname);
				
				var effects = character.GetComponent<Effects>();
				effects.NicknameRenderers = meshNickname.GetComponentsInChildren<Renderer>().ToList();
			}
		}

		private GameObject InstantiateBackground(Level prevLevel, Level newLevel, float time)
		{
			var levelNumber = LevelRepository.Instance.CurrentLevelNumber % Backgrounds.Instance.LevelsBackgrounds[Backgrounds.Instance.LevelsBackgrounds.Count-1].ToLevel;
		
			var backgroundPrefab =
				Backgrounds.Instance.GetBackgroundByLevel(levelNumber != 0 ? 
				levelNumber : Backgrounds.Instance.LevelsBackgrounds[Backgrounds.Instance.LevelsBackgrounds.Count-1].ToLevel);

			var background = Instantiate(backgroundPrefab, CurrLocationPos, backgroundPrefab.transform.rotation);

			Material currMat;

			if (prevLevel != null)
			{
				var prevMat = this.currBackground.GetComponent<Renderer>().material;
			
				currMat = Instantiate(prevMat);

				StartCoroutine(LerpBackground(currMat, prevMat, newLevel.BackgroundMaterial, time));
			}
			else
			{
				currMat = Instantiate(newLevel.BackgroundMaterial);
			}
			
			background.GetComponent<Renderer>().material = currMat;

			var trs = this.currLocation.GetComponentsInChildren<Transform>();

			foreach (var tr in trs)
			{
				if (tr.name.Equals("base"))
				{
					tr.GetComponent<Renderer>().material =
						currMat;
						
					break;
				}
			}

			return background;
		}

		private IEnumerator LerpBackground(Material mat1, Material mat2, Material newMat, float time)
		{
			yield return new WaitForSeconds(0.5f);

			var cameraController = FindObjectOfType<CameraController>();

			var camera = cameraController.GetComponent<Camera>();

			var currT = time;

			var mat1ColorFog = mat1.GetColor(ColorFog);
			var mat1Color1F = mat1.GetColor(Color1F);
			var mat1Color1B = mat1.GetColor(Color1B);
			var mat1Color1L = mat1.GetColor(Color1L);
			var mat1Color1R = mat1.GetColor(Color1R);
			var mat1Color1T = mat1.GetColor(Color1T);
			var mat1Color1D = mat1.GetColor(Color1D);
			
			var mat2Color1F = mat2.GetColor(Color1F);
			var mat2Color1B = mat2.GetColor(Color1B);
			var matColor1L = mat2.GetColor(Color1L);
			var mat2Color1R = mat2.GetColor(Color1R);
			var mat2Color1T = mat2.GetColor(Color1T);
			var mat2Color1D = mat2.GetColor(Color1D);
			var mat2ColorFog = mat2.GetColor(ColorFog);
			
			
			while (currT > 0f)
			{
				var lerp = 1 - currT / time;
				camera.backgroundColor = Color.Lerp(mat1ColorFog, newMat.GetColor(ColorFog), lerp);
			
				mat1.SetColor(Color1F, Color.Lerp(mat1Color1F, newMat.GetColor(Color1F), lerp));
				mat1.SetColor(Color1B, Color.Lerp(mat1Color1B, newMat.GetColor(Color1B), lerp));
				mat1.SetColor(Color1L, Color.Lerp(mat1Color1L, newMat.GetColor(Color1L), lerp));
				mat1.SetColor(Color1R, Color.Lerp(mat1Color1R, newMat.GetColor(Color1R), lerp));
				mat1.SetColor(Color1T, Color.Lerp(mat1Color1T, newMat.GetColor(Color1T), lerp));
				mat1.SetColor(Color1D, Color.Lerp(mat1Color1D, newMat.GetColor(Color1D), lerp));
				mat1.SetColor(ColorFog, Color.Lerp(mat1ColorFog, newMat.GetColor(ColorFog), lerp));
				
				mat2.SetColor(Color1F, Color.Lerp(mat2Color1F, newMat.GetColor(Color1F), lerp));
				mat2.SetColor(Color1B, Color.Lerp(mat2Color1B, newMat.GetColor(Color1B), lerp));
				mat2.SetColor(Color1L, Color.Lerp(matColor1L, newMat.GetColor(Color1L), lerp));
				mat2.SetColor(Color1R, Color.Lerp(mat2Color1R, newMat.GetColor(Color1R), lerp));
				mat2.SetColor(Color1T, Color.Lerp(mat2Color1T, newMat.GetColor(Color1T), lerp));
				mat2.SetColor(Color1D, Color.Lerp(mat2Color1D, newMat.GetColor(Color1D), lerp));
				mat2.SetColor(ColorFog, Color.Lerp(mat2ColorFog, newMat.GetColor(ColorFog), lerp));
				
				currT -= Time.deltaTime;

				yield return null;
			}
		}

		private void ApplyMaterials(Level level)
		{
			var trs = this.currLocation.GetComponentsInChildren<Transform>();

			foreach (var tr in trs)
			{
				if (tr.name.Equals("floor"))
				{
					tr.GetComponent<Renderer>().materials = level.ColorScheme.FloorMaterials;
				}
				
				if (tr.name.Equals("wall"))
				{
					tr.GetComponent<Renderer>().materials = level.ColorScheme.WallMaterials;
				}
				
				if (tr.name.Equals("bridge"))
				{
					tr.GetComponent<Renderer>().materials = level.ColorScheme.BridgeMaterials;
				}
				
				if (tr.name.Equals("border"))
				{
					tr.GetComponent<Renderer>().materials = new[] {level.ColorScheme.WallMaterials[0]};
				}
			}
		}

		private MeshNickname InstantiateMeshNickname(string nickname, Transform target, Vector3 offset)
		{
			var meshNickname = Instantiate(this.meshNicknamePrefab);
			
			meshNickname.SetNickname(nickname);
			meshNickname.Target = target;
			meshNickname.Offset = offset;

			return meshNickname;
		}

		private void SpawnBots(int count, GameObject[] respawns) 
		{
			var bots = new Character[count];
		
			for (var i = 0; i < this.hideAndSeekGameMode.PlayersCount; i++)
			{
				bots[i] = SpawnBotCharacter(this.characterPrefab, respawns[i + 1].transform.position, respawns[i + 1].transform.rotation);
			}

			SetRandomProfileInfo(bots);
		}
		
		private void LoadNicknames()
		{
			if (this.nicknamesFile == null)
			{
				return;
			}

			using (var sr = new StreamReader(new MemoryStream(this.nicknamesFile.bytes)))
			{
				this.nicknames = sr.ReadToEnd().Split(new[]{"\n"}, StringSplitOptions.RemoveEmptyEntries);
			}
		}

		private void LoadLevelsInfo()
		{
			if (PlayerPrefs.HasKey(LevelsInfoKey))
			{
				var levelsInfoJson = PlayerPrefs.GetString(LevelsInfoKey);

				this.levelInfoWrapper = JsonUtility.FromJson<LevelInfoWrapper>(levelsInfoJson);
			}
			else
			{
				this.levelInfoWrapper = new LevelInfoWrapper {LevelsInfo = new List<LevelInfo>()};
			}
		}

		private void SaveLevelsInfo()
		{
			var levelsInfoJson = JsonUtility.ToJson(this.levelInfoWrapper, true);
			
			PlayerPrefs.SetString(LevelsInfoKey, levelsInfoJson);
			PlayerPrefs.Save();
		}
		
		private void OnActivePerkPickedUp(ActivePerkName activePerkName, ActivePerkController activePerkController)
		{
			var levelInfo = 
			 this.levelInfoWrapper.LevelsInfo.SingleOrDefault(l => l.LevelNumber == LevelRepository.Instance.CurrentLevelNumber);

			if (levelInfo != null)
			{
				levelInfo.IsActivePerkAvailable = 0;
			}
		}

		private void LoadColorSchemes(Level level, int levelNumber)
		{
			var levelInfo =
				this.levelInfoWrapper.LevelsInfo.SingleOrDefault(l =>
					l.LevelNumber == levelNumber);

			if (levelInfo != null)
			{
				if (!string.IsNullOrEmpty(levelInfo.ColorSchemeName))
				{
					level.ColorScheme = level.ColorSchemes.First(s => s.name.Equals(levelInfo.ColorSchemeName));

					level.BackgroundMaterial =
						level.ColorScheme.BackgroundMaterials.First(
							m => m.name.Equals(levelInfo.BackgroundMaterialName));
				}
				else
				{
					level.ColorScheme = level.ColorSchemes[Random.Range(0, level.ColorSchemes.Length)];

					if (level.ColorScheme != null)
					{
						level.BackgroundMaterial =
							level.ColorScheme.BackgroundMaterials[
								Random.Range(0, level.ColorScheme.BackgroundMaterials.Length)];
					}

					levelInfo.ColorSchemeName = level.ColorScheme.name;
					levelInfo.BackgroundMaterialName = level.BackgroundMaterial.name;
				}
			}
			else
			{
				level.ColorScheme = level.ColorSchemes[Random.Range(0, level.ColorSchemes.Length)];

				if (level.ColorScheme != null)
				{
					level.BackgroundMaterial =
						level.ColorScheme.BackgroundMaterials[
							Random.Range(0, level.ColorScheme.BackgroundMaterials.Length)];
				}
			}
		}

		private void InitLevelInfo(int levelNumber)
		{
			var levelInfo =
				this.levelInfoWrapper.LevelsInfo.SingleOrDefault(l =>
					l.LevelNumber == levelNumber);

			if (levelInfo == null)
			{
				levelInfo = new LevelInfo
				{
					LevelNumber = levelNumber,
					IsActivePerkAvailable = 1,
					IsActivePerkShown = 0,
					Rotation = -300f
				};

				this.levelInfoWrapper.LevelsInfo.Add(levelInfo);
			}
		}

		private void RandomNextActivePerkLevel()
		{
			NextActivePerkLevel = LevelRepository.Instance.CurrentLevelNumber + Random.Range(1, 4);
		}
	}
}