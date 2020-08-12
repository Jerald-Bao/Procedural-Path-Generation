//
// Dedicated to all my Patrons on Patreon,
// as a thanks for your continued support 💖
//
// Source code © Freya Holmér, 2019
// This code is provided exclusively to supporters,
// under the Attribution Assurance License
// "https://tldrlegal.com/license/attribution-assurance-license-(aal)"
// 
// You can basically do whatever you want with this code,
// as long as you include this license and credit me for it,
// in both the source code and any released binaries using this code
//
// Thank you so much again <3
//
// Freya
//

using UnityEngine;

[CreateAssetMenu]
public class Mesh2D : ScriptableObject {

	// A 2D vertex
	[System.Serializable]
	public class Vertex {
		public Vector2 point;
		public Vector2 normal;
		public float u; // UVs, but like, not V :thinking_face:
		// vertex colors
		// tangents
	}

	// Just like 3D meshes define connectivity with triangles by triplets of indices,
	// our 2D mesh will define connectivity with lines by pairs of indices
	public int[] lineIndices;
	public Vertex[] vertices;

	public int VertexCount => vertices.Length;
	public int LineCount => lineIndices.Length; // Triangle count equivalent of a 2D mesh

	// Total length covered by the U coordinates in world space
	// Used for making sure the texture has the correct aspect ratio
	public float CalcUspan() {
		float dist = 0;
		for( int i = 0; i < LineCount; i+=2 ) {
			Vector2 a = vertices[lineIndices[i]].point;
			Vector2 b = vertices[lineIndices[i+1]].point;
			dist += ( a - b ).magnitude;
		}
		return dist;
	}

}
