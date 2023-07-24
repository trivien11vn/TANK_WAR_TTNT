using Unity.Entities;
using UnityEngine;

namespace Component
{
    public class PlayerAuthoring : MonoBehaviour
    {
        public GameObject player;
        
        public class PlayerBaker : Baker<PlayerAuthoring>
        {
            public override void Bake(PlayerAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new Player
                {
                    Playerr = GetEntity(authoring.player, TransformUsageFlags.Dynamic)
                });
            }
        }
    }
}