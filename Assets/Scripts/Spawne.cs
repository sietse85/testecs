using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Spawne : MonoBehaviour
{
    public Mesh currentMesh;
    public Mesh sphere;
    public Material material;
    EntityManager m;

    private Entity en1;
    private Entity en2;

    public Button parent;
    public Button move;
    public Button destroy;
    
    private void Start()
    {
        m  = World.Active.EntityManager;

        
        LinkedEntityGroup group = new LinkedEntityGroup();
        
        en1 = CraftEntity(new float3(Random.Range(-1f,1f), 0, Random.Range(-1f, 1f)));
        en2 = CraftSphere(new float3(Random.Range(-1f,1f), 0, Random.Range(-1f, 1f)));
        
        parent.onClick.AddListener(MergeEntitiesTogether(en1, en2));
        move.onClick.AddListener(MoveEntities());
        destroy.onClick.AddListener(delegate { m.DestroyEntity(en1); });
        
    }

    public UnityAction MergeEntitiesTogether(Entity parent, Entity child)
    {
        return () =>
        {
            if (!m.HasComponent(child, typeof(Parent)))
            {
                m.AddComponentData(child, new Parent { Value = parent });
                m.AddComponentData(child, new LocalToParent() );

                DynamicBuffer<LinkedEntityGroup> buf = m.GetBuffer<LinkedEntityGroup>(parent);
                buf.Add(child);
            }
        };
    }

    public UnityAction MoveEntities()
    {
        return () =>
        {
            m.SetComponentData(en1, new Translation
            {
                Value = new float3(Random.Range(-2f, 2f), 0, -2f)
            });
        };
    }

    public Entity CraftEntity(float3 pos)
    {
        Entity e = m.CreateEntity();
        
        m.AddSharedComponentData(e, new RenderMesh
        {
            mesh = currentMesh,
            material = material,
            subMesh = 0,
            layer = 8,
        });
        m.AddComponent(e, typeof(LocalToWorld));
        m.AddComponentData(e, new Translation { Value = pos});
        m.AddBuffer<LinkedEntityGroup>(e);
        
        DynamicBuffer<LinkedEntityGroup> buf = m.GetBuffer<LinkedEntityGroup>(e);
        buf.Add(e);
        return e;
    }

    public Entity CraftSphere(float3 pos)
    {
        Entity e = m.CreateEntity();
        
        m.AddSharedComponentData(e, new RenderMesh
        {
            mesh = sphere,
            material = material,
            subMesh = 0,
            layer = 8,
        });
        m.AddComponent(e, typeof(LocalToWorld));
        m.AddComponentData(e, new Translation { Value = pos});

        return e;
    }
}


