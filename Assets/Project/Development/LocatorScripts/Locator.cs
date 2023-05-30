using System;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Development.LocatorScripts
{
	public abstract class Locator<T>: MonoBehaviour
	{
		public Action<Locator<T>, List<T>> OnLocatorFound;
	
		public float Rate { get; set; }
		
		public List<T> Objects { get; set; }
		
		public LayerMask LayerMask { get; set; }

		private float currentFindTimer;
		
		
		
		
		private void Update()
		{
			this.currentFindTimer += Time.deltaTime;

			if (this.currentFindTimer >= this.Rate)
			{
				Find();

				this.currentFindTimer = 0f;
			}
		}




		public abstract bool Find(T transform);
		
		public List<T> Find()
		{
			List<T> foundObjects = null;
		
			foreach (var obj in this.Objects)
			{
				if (Find(obj))
				{
					if (foundObjects == null)
					{
						foundObjects = new List<T>();
					}
				
					foundObjects.Add(obj);
				}
			}

			if (foundObjects != null)
			{
				OnLocatorFound?.Invoke(this, foundObjects);
			}

			return foundObjects;
		}

		public void SetActive(bool active)
		{
			this.enabled = active;
		}
	}
}