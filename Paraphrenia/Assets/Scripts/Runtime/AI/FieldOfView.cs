using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FieldOfView : MonoBehaviour {
	[Range(0, 50)] public float viewRadius = 30;
	[Range(0,360)] public float viewAngle = 75;
	public LayerMask targetMask;
	public LayerMask obstacleMask;
	[HideInInspector] public List<Transform> visibleTargets = new List<Transform>();

	[SerializeField] private float _tickDelay = 0.1f;
	[SerializeField] private bool _drawFieldOfView = true;
	[SerializeField] private float _meshResolution = 10;
	[SerializeField] private float _edgeDistanceThreshold = 5;
	[SerializeField] private int _edgeResolveIterations = 5;
	[SerializeField] private MeshFilter _viewMeshFilter;

	private Mesh _viewMesh;

	private void Start()
	{
		_viewMesh = new Mesh ();
		_viewMesh.name = "View Mesh";
		_viewMeshFilter.mesh = _viewMesh;

		StartCoroutine ("FindTargetsWithDelay", _tickDelay);
	}

	private void LateUpdate()
	{
		if(_drawFieldOfView) DrawFieldOfView();
	}

	IEnumerator FindTargetsWithDelay(float delay)
	{
		while (true)
		{
			yield return new WaitForSeconds (delay);
			FindVisibleTargets ();
		}
	}

	// For each target within a sphere of this object, check if the target falls within the field of view angle, and whether there is direct line of sight.
	private void FindVisibleTargets()
	{
		visibleTargets.Clear ();
		Collider[] targetsInViewRadius = Physics.OverlapSphere (transform.position, viewRadius, targetMask);

		foreach (Collider target in targetsInViewRadius)
        {
			Vector3 directionToTarget = (target.transform.position - transform.position).normalized;
			
			if (Vector3.Angle(transform.forward, directionToTarget) > viewAngle / 2)
			{
				return; // Target is outside of view angle
			}
			float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);
			if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstacleMask))
			{
				visibleTargets.Add(target.transform); // Target must not be blocked by obstacle
			}
		}
	}

	private void DrawFieldOfView()
	{
		// Calculate the amount of rays to cast in the FoV drawing algorithm
		int stepCount = Mathf.RoundToInt(viewAngle * _meshResolution);
		float stepAngleSize = viewAngle / stepCount;
		List<Vector3> viewPoints = new List<Vector3> ();
		ViewCastInfo oldViewCast = new ViewCastInfo ();

		for (int i = 0; i <= stepCount; i++)
		{
			float angle = transform.eulerAngles.y - viewAngle / 2 + stepAngleSize * i;
			ViewCastInfo newViewCast = ViewCast (angle);

			if (i > 0)
			{
				// See if we can find a corner of a mesh that we can use to construct or visualization mesh
				bool edgeDistanceThresholdExceeded = Mathf.Abs(oldViewCast.distance - newViewCast.distance) > _edgeDistanceThreshold;
				if (oldViewCast.hit != newViewCast.hit || (oldViewCast.hit && newViewCast.hit && edgeDistanceThresholdExceeded))
				{
					EdgeInfo edge = FindEdge (oldViewCast, newViewCast);
					// Construct part of the mesh if a corner was found
					if (edge.pointA != Vector3.zero) viewPoints.Add (edge.pointA);
					if (edge.pointB != Vector3.zero) viewPoints.Add (edge.pointB);
				}
			}
			viewPoints.Add (newViewCast.point);
			oldViewCast = newViewCast;
		}

		// Recalculate mesh data based on the generated points generated
		int vertexCount = viewPoints.Count + 1;
		Vector3[] vertices = new Vector3[vertexCount];
		int[] triangles = new int[(vertexCount-2) * 3];

		vertices [0] = Vector3.zero;
		for (int i = 0; i < vertexCount - 1; i++)
		{
			vertices [i + 1] = transform.InverseTransformPoint(viewPoints [i]);

			if (i < vertexCount - 2)
			{
				triangles [i * 3] = 0;
				triangles [i * 3 + 1] = i + 1;
				triangles [i * 3 + 2] = i + 2;
			}
		}

		// Reconstruct the procedural mesh
		_viewMesh.Clear ();
		_viewMesh.vertices = vertices;
		_viewMesh.triangles = triangles;
		_viewMesh.RecalculateNormals ();
	}


	EdgeInfo FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast) {
		float minAngle = minViewCast.angle;
		float maxAngle = maxViewCast.angle;
		Vector3 minPoint = Vector3.zero;
		Vector3 maxPoint = Vector3.zero;

		// Takes two angles in between an object corner was found. Then, checks if a raycast in between the two hits the object, and updates the angles accordingly. Through only a handful of itterations, this will accurately find the corner of the object
		for (int i = 0; i < _edgeResolveIterations; i++) 
		{
			float angle = (minAngle + maxAngle) / 2;
			ViewCastInfo newViewCast = ViewCast (angle);

			bool edgeDistanceThresholdExceeded = Mathf.Abs (minViewCast.distance - newViewCast.distance) > _edgeDistanceThreshold;
			if (newViewCast.hit == minViewCast.hit && !edgeDistanceThresholdExceeded)
			{
				minAngle = angle;
				minPoint = newViewCast.point;
			} else {
				maxAngle = angle;
				maxPoint = newViewCast.point;
			}
		}

		return new EdgeInfo (minPoint, maxPoint);
	}


	ViewCastInfo ViewCast(float globalAngle)
	{
		Vector3 direction = DirectionFromAngle(globalAngle, true);
		RaycastHit hit;

		if (Physics.Raycast (transform.position, direction, out hit, viewRadius, obstacleMask))
		{
			return new ViewCastInfo (true, hit.point, hit.distance, globalAngle);
		} else {
			return new ViewCastInfo (false, transform.position + direction * viewRadius, viewRadius, globalAngle);
		}
	}

	public Vector3 DirectionFromAngle(float angleInDegrees, bool angleIsGlobal)
	{
		if (!angleIsGlobal) angleInDegrees += transform.eulerAngles.y;
		return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad),0,Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
	}

	public struct ViewCastInfo
	{
		public bool hit;
		public Vector3 point;
		public float distance;
		public float angle;

		public ViewCastInfo(bool _hit, Vector3 _point, float _distance, float _angle)
		{
			hit = _hit;
			point = _point;
			distance = _distance;
			angle = _angle;
		}
	}

	public struct EdgeInfo
	{
		public Vector3 pointA;
		public Vector3 pointB;

		public EdgeInfo(Vector3 _pointA, Vector3 _pointB)
		{
			pointA = _pointA;
			pointB = _pointB;
		}
	}

}