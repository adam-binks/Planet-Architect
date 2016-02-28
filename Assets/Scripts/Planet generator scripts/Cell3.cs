using UnityEngine;
using System.Collections;
using System.Linq;
using MIConvexHull;

/// <summary>
/// A vertex is a simple class that stores the postion of a point, node or vertex.
/// </summary>
public class Cell3 : TriangulationCell<Vertex3, Cell3>
{

	Vector3? circumCenter;
	
	public Vector3 Circumcenter {
		get {
			circumCenter = circumCenter ?? GetCircumcenter();
			return circumCenter.Value;
		}
	}
	
	public Cell3 ()
	{
		
	}

	double LengthSquared (double[] v)
	{
		double norm = 0;
		for (int i = 0; i < v.Length; i++) {
			var t = v [i];
			norm += t * t;
		}
		return norm;
	}

	private double MINOR(double[,] m, int r0, int r1, int r2, int c0, int c1, int c2)
	{
		return  m[r0,c0] *(m[r1,c1] * m[r2,c2] - m[r2,c1] * m[r1,c2]) -
				m[r0,c1] *(m[r1,c0] * m[r2,c2] - m[r2,c0] * m[r1,c2]) +
				m[r0,c2] *(m[r1,c0] * m[r2,c1] - m[r2,c0] * m[r1,c1]);
	}
	
	private double Determinant(double[,] m)
	{
		return (m[0,0] * MINOR(m, 1, 2, 3, 1, 2, 3) -
		        m[0,1] * MINOR(m, 1, 2, 3, 0, 2, 3) +
		        m[0,2] * MINOR(m, 1, 2, 3, 0, 1, 3) -
		        m[0,3] * MINOR(m, 1, 2, 3, 0, 1, 2));
	}

	Vector3 GetCircumcenter ()
	{
		// From MathWorld: http://mathworld.wolfram.com/Circumsphere.html

		var points = Vertices;
		
		double[,] m = new double[4, 4];
		
		// x, y, z, 1
		for (int i = 0; i < 4; i++) {
			m[i, 0] = points[i].x;
			m[i, 1] = points[i].y;
			m[i, 2] = points[i].z;
			m[i, 3] = 1;
		}
		var a = Determinant(m);
		
		// size, y, z, 1
		for (int i = 0; i < 4; i++) {
			m[i, 0] = LengthSquared (points[i].Position);
		}
		var dx = Determinant(m);
		
		// size, x, z, 1
		for (int i = 0; i < 4; i++) {
			m[i, 1] = points[i].x;
		}
		var dy = -Determinant(m);

		// size, x, y, 1
		for (int i = 0; i < 4; i++) {
			m[i, 2] = points[i].y;
		}
		var dz = Determinant(m);
		
		// size, x, y, z
//		for (int i = 0; i < 4; i++) {
//			m[i, 3] = points[i].z;
//		}
//		var c = Determinant(m);
		
		var s = -1.0 / (2.0 * a);
		//var r = System.Math.Abs(s) * System.Math.Sqrt(dx * dx + dy * dy + dz * dz - 4 * a * c);

		return new Vector3((float)(s * dx), (float)(s * dy), (float)(s * dz));
	}
	  
}

















