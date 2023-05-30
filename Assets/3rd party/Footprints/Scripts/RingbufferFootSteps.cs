using UnityEngine;

namespace Footprints.Scripts
{
    public class RingbufferFootSteps : MonoBehaviour
    {
        [field: SerializeField]
        public float Delta { get; set; }
        [field: SerializeField]
        public float Gap { get; set; }
        
        private int dir;
        private Vector3 lastEmit;
        private ParticleSystem system;




        private void Awake()
        {
            this.system = GetComponent<ParticleSystem>();
        
            this.dir = 1;
        }

        private void Start()
        {
            this.lastEmit = this.transform.position;
        }

        private void Update()
        {
            if (Vector3.Distance(this.lastEmit, this.transform.position) > this.Delta)
            {
                var pos = this.transform.position + (this.transform.right * this.Gap * this.dir);
                
                this.dir *= -1;
                
                var ep = new ParticleSystem.EmitParams
                {
                    position = pos, rotation = this.transform.rotation.eulerAngles.y
                };
                
                this.system.Emit(ep, 1);
                this.lastEmit = this.transform.position;
            }
        }
    }
}
