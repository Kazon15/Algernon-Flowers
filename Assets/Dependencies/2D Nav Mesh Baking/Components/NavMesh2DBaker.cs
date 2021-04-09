using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Wokarol.NavMesh2D
{
    [RequireComponent(typeof(NavMeshSurface)), AddComponentMenu("Navigation/Nav Mesh 2D Baker")]
    public class NavMesh2DBaker : MonoBehaviour
    {
        private const int collidersDepth = 50;

        [SerializeField]
        private Vector2 groundSize = new Vector2(40, 40);

        private readonly Vector3 Box = new Vector3(1, 1, 1);

        private void OnDrawGizmosSelected() =>
            Gizmos.DrawWireCube(transform.position, (Vector3) groundSize + Vector3.forward * 5);

        public void Bake()
        {
            var createdObjects = new List<GameObject>();

            // Ground creation
            var ground = new GameObject("NavMeshGround");
            ground.transform.SetPositionAndRotation(transform.position, Quaternion.identity);
            var groundCollider = ground.AddComponent<BoxCollider>();
            groundCollider.size = groundSize;
            createdObjects.Add(ground);

            ConvertColliders(createdObjects);
            // Baking NavMesh
            var surfaces = GetComponents<NavMeshSurface>();
            foreach(var surface in surfaces)
            {
                if(surface.useGeometry != NavMeshCollectGeometry.PhysicsColliders)
                {
                    surface.useGeometry = NavMeshCollectGeometry.PhysicsColliders;
                    Debug.Log("NavMesh should be set to use Physics Colliders, I've corrected it for you");
                }

                surface.BuildNavMesh();
            }

            // Clearing after process
            foreach(var createdObject in createdObjects) DestroyImmediate(createdObject);

            Debug.Log("NavMesh 2D Baking complete");
        }

        private void ConvertColliders(List<GameObject> createdObjects)
        {
            ConvertBoxes(createdObjects);
            ConvertCircles(createdObjects);
            ConvertCapsules(createdObjects);
            ConvertPolygons(createdObjects);
            ConvertComposites(createdObjects);
            ConvertEdgeColliders(createdObjects);
        }

        private void ConvertComposites(List<GameObject> createdObjects)
        {
            var composites2D = FindObjectsOfType<CompositeCollider2D>();
            foreach(var composite in composites2D)
            {
                if(Isexcluded(composite.gameObject)) continue;
                ConvertCoposite(composite, createdObjects);
            }
        }

        private void ConvertPolygons(List<GameObject> createdObjects)
        {
            var polygons2D = FindObjectsOfType<PolygonCollider2D>();
            foreach(var polygon in polygons2D)
            {
                if(Isexcluded(polygon.gameObject)) continue;
                for(var i = 0; i < polygon.pathCount; i++)
                {
                    {
                        var path = polygon.GetPath(i);
                        var mesh = Collider2DTo3DUtils.GetMeshFromPath(path, true);
                        var meshCollider = GetObjectCopy(polygon.gameObject, createdObjects)
                           .AddComponent<MeshCollider>();
                        meshCollider.sharedMesh = mesh;
                    }
                }
            }
        }

        private void ConvertBoxes(List<GameObject> createdObjects)
        {
            var colliderZSize = Vector3.forward * collidersDepth;
            var boxColliders2D = FindObjectsOfType<BoxCollider2D>();
            foreach(var boxCollider in boxColliders2D)
            {
                if(Isexcluded(boxCollider.gameObject)) continue;
                var col = GetObjectCopy(boxCollider.gameObject, createdObjects).AddComponent<BoxCollider>();
                col.size = (Vector3) boxCollider.size + colliderZSize;
                col.center = boxCollider.offset;
            }
        }

        private void ConvertCircles(List<GameObject> createdObjects)
        {
            var circleCollider2D = FindObjectsOfType<CircleCollider2D>();
            foreach(var circleCollider in circleCollider2D)
            {
                if(Isexcluded(circleCollider.gameObject)) continue;
                var col = GetObjectCopy(circleCollider.gameObject, createdObjects)
                   .AddComponent<CapsuleCollider>();
                col.radius = circleCollider.radius;
                col.center = circleCollider.offset;
                col.height = collidersDepth;
                col.direction = 2;
            }
        }

        private void ConvertCapsules(List<GameObject> createdObjects)
        {
            var capsuleColiders2D = FindObjectsOfType<CapsuleCollider2D>();
            foreach(var capsuleCollider in capsuleColiders2D)
            {
                if(Isexcluded(capsuleCollider.gameObject)) continue;
                var obj = GetObjectCopy(capsuleCollider.gameObject, createdObjects);
                switch(capsuleCollider.direction)
                {
                    case CapsuleDirection2D.Vertical:
                    {
                        var correctedSize = capsuleCollider.size;
                        correctedSize.y = Mathf.Clamp(correctedSize.y, correctedSize.x, float.MaxValue);

                        // Box collider
                        var boxCol = obj.AddComponent<BoxCollider>();
                        boxCol.center = capsuleCollider.offset;
                        boxCol.size = new Vector3(correctedSize.x, correctedSize.y - correctedSize.x,
                                                  collidersDepth);

                        // Top capsule collider
                        var topCapsCol = obj.AddComponent<CapsuleCollider>();
                        var offset = capsuleCollider.offset;
                        var size = boxCol.size;
                        topCapsCol.center = new Vector3(offset.x, offset.y - size.y / 2);
                        topCapsCol.radius = size.x / 2;
                        topCapsCol.height = collidersDepth;
                        topCapsCol.direction = 2;

                        // Top capsule collider
                        var bottomCapsCol = obj.AddComponent<CapsuleCollider>();
                        var offset1 = capsuleCollider.offset;
                        var size1 = boxCol.size;
                        bottomCapsCol.center = new Vector3(offset1.x, offset1.y + size1.y / 2);
                        bottomCapsCol.radius = size1.x / 2;
                        bottomCapsCol.height = collidersDepth;
                        bottomCapsCol.direction = 2;
                        break;
                    }
                    case CapsuleDirection2D.Horizontal:
                    {
                        var correctedSize = capsuleCollider.size;
                        correctedSize.x = Mathf.Clamp(correctedSize.x, correctedSize.y, float.MaxValue);

                        // Box collider
                        var boxCol = obj.AddComponent<BoxCollider>();
                        boxCol.center = capsuleCollider.offset;
                        boxCol.size = new Vector3(correctedSize.x - correctedSize.y, correctedSize.y,
                                                  collidersDepth);

                        // Top capsule collider
                        var topCapsCol = obj.AddComponent<CapsuleCollider>();
                        var offset = capsuleCollider.offset;
                        var size = boxCol.size;
                        topCapsCol.center = new Vector3(offset.x - size.x / 2, offset.y);
                        topCapsCol.radius = size.y / 2;
                        topCapsCol.height = collidersDepth;
                        topCapsCol.direction = 2;

                        // Bottom capsule collider
                        var bottomCapsCol = obj.AddComponent<CapsuleCollider>();
                        var offset1 = capsuleCollider.offset;
                        var size1 = boxCol.size;
                        bottomCapsCol.center = new Vector3(offset1.x + size1.x / 2, offset1.y);
                        bottomCapsCol.radius = size1.y / 2;
                        bottomCapsCol.height = collidersDepth;
                        bottomCapsCol.direction = 2;
                        break;
                    }
                    default: return;
                }
            }
        }

        private void ConvertEdgeColliders(List<GameObject> createdObjects)
        {
            var edgeColliders = FindObjectsOfType<EdgeCollider2D>();
            foreach(var edgeCollider in edgeColliders)
            {
                if(Isexcluded(edgeCollider.gameObject)) continue;
                ConvertEdgeCollider(edgeCollider, createdObjects);
            }
        }

        private void ConvertCoposite(CompositeCollider2D compositeCollider, List<GameObject> createdObjects)
        {
            if(compositeCollider.edgeRadius < Mathf.Epsilon)
            {
                for(var i = 0; i < compositeCollider.pathCount; i++)
                {
                    var path = new Vector2[compositeCollider.GetPathPointCount(i)];
                    compositeCollider.GetPath(i, path);

                    var mesh = Collider2DTo3DUtils.GetMeshFromPath(path, true);
                    var obj = GetObjectCopy(compositeCollider.gameObject, createdObjects);
                    var meshCollider = obj.AddComponent<MeshCollider>();
                    meshCollider.sharedMesh = mesh;
                }
            }
            else
            {
                for(var i = 0; i < compositeCollider.pathCount; i++)
                {
                    var path = new Vector2[compositeCollider.GetPathPointCount(i)];
                    compositeCollider.GetPath(i, path);
                    ConvertPath(compositeCollider.gameObject, compositeCollider.edgeRadius, createdObjects,
                                path, true);
                }
            }
        }

        private void ConvertEdgeCollider(EdgeCollider2D edgecCollider, List<GameObject> createdObjects)
        {
            if(edgecCollider.edgeRadius < Mathf.Epsilon)
            {
                var path = edgecCollider.points;

                var mesh = Collider2DTo3DUtils.GetMeshFromPath(path, false);
                var obj = GetObjectCopy(edgecCollider.gameObject, createdObjects);
                var meshCollider = obj.AddComponent<MeshCollider>();
                meshCollider.sharedMesh = mesh;
            }
            else
            {
                var path = edgecCollider.points;
                ConvertPath(edgecCollider.gameObject, edgecCollider.edgeRadius, createdObjects, path, false);
            }
        }

        private void ConvertPath(GameObject gameObj, float edgeRadius, List<GameObject> createdObjects,
                                 IReadOnlyList<Vector2> path, bool loop)
        {
            // Converting point to 3D
            var obj = GetObjectCopy(gameObj, createdObjects);
            obj.transform.localPosition = Vector3.zero;
            foreach(var t in path)
            {
                // Create circle
                var capsule = obj.AddComponent<CapsuleCollider>();
                capsule.direction = 2;
                capsule.center = t;
                capsule.radius = edgeRadius;
                capsule.height = 20;
            }

            // Converting Edges to 3D
            for(var j = 0; j < path.Count; j++)
            {
                if(j == path.Count - 1 && !loop) continue;
                Collider2DTo3DUtils.CreateLine(path[j], path[(j + 1) % path.Count], edgeRadius * 2,
                                               obj.transform);
            }
        }

        private GameObject GetObjectCopy(GameObject original, List<GameObject> createdObjects)
        {
            var obj = new GameObject($"{original.name}_NMClone");
            obj.transform.parent = original.transform;
            var newPos = original.transform.position;
            newPos.z = 0;
            obj.transform.SetPositionAndRotation(newPos, original.transform.rotation);
            obj.transform.localScale = Box;
            createdObjects.Add(obj);
            return obj;
        }

        private static bool Isexcluded(GameObject obj) =>
            !obj.isStatic || obj.GetComponent<NavMeshObstacle>() != null;
    }
}