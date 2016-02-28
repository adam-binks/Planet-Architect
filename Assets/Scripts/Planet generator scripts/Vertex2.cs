using UnityEngine;
using System.Collections;
using MIConvexHull;

public class Vertex2 : IVertex
{

	public double[] Position { get; set; }
	
	public double x { get { return Position[0]; } }
	public double y { get { return Position[1]; } }
	
	public Vertex2(double x, double y)
	{
		Position = new double[] { x, y };
	}

	public Vector2 ToVector2() {
		return new Vector2((float)Position[0], (float)Position[1]);
	}
	
	public Vector3 ToVector3() {
		return new Vector3((float)Position[0], (float)Position[1], 0.0f);
	}
}
