using UnityEngine;
using System.Collections;

public class Landscape : MonoBehaviour
{
	public float microScale			= 0.2f;
	public float megaScale			= 50.0f;
	public float verticalMicroStretch = 25.0f;
	public float verticalMegaStretch = 150.0f;
	public float cylinderRadius 	= 1050.0f;
	public float cylinderLength 	= 3700.0f;
	public Material terrainMtl;
	public int treeCount	 		= 100;
	public GameObject treePrefab1;
	
	// Internal vars
	private Mesh emptyMesh;
	private Texture2D texture;
	private Vector3[] vertices;
	private Vector2[] uv;
	//private Vector3[] normals;
	private float[] heightfield;
	private int[] triangles;
	private int width 	= 128;
	private int height 	= 128;
	private int texSize	= 256;

	// Use this for initialization
	void Start ()
	{
		Application.targetFrameRate = 30;
		
		// Create the game object containing the renderer
		gameObject.AddComponent<MeshFilter>();
		gameObject.AddComponent<MeshRenderer>();
		gameObject.AddComponent<MeshCollider>();
		
		emptyMesh = new Mesh();	// empty mesh
		emptyMesh.vertices = new Vector3[3];
		emptyMesh.triangles = new int[3];
		emptyMesh.vertices[0] = Vector3.zero;
		emptyMesh.vertices[1] = Vector3.forward;
		emptyMesh.vertices[2] = Vector3.left;
		emptyMesh.triangles[0] = 0;
		emptyMesh.triangles[1] = 1;
		emptyMesh.triangles[2] = 2;
		
		// Allocate all storage for mesh, since size will not vary on regeneration - only content				
		vertices 	= new Vector3[height * width];
		uv 			= new Vector2[height * width];
		//normals 	= new Vector3[height * width];
		heightfield = new float[(height+2) * (width+2)];
		triangles 	= new int[(height - 1) * (width - 1) * 6];
		texture 	= new Texture2D( texSize, texSize, TextureFormat.RGB24, false, true);
		texture.filterMode = FilterMode.Bilinear;
	
		renderer.castShadows	= false;
		//renderer.material.color = Color.white;
		//renderer.material.mainTexture = texture;
		//renderer.material.mainTexture.wrapMode = TextureWrapMode.Clamp;
		renderer.material		= terrainMtl;
		
		GenerateMesh();
	}
	
	float GetFractal( float x, float y )
	{	
		// Two octaves is the minimum for anything fractal-looking
		float pixelHeight = verticalMicroStretch * Mathf.PerlinNoise( x*microScale+1000.0f, y*microScale+1000.0f ) +
							verticalMegaStretch  * Mathf.PerlinNoise( (x*microScale/megaScale)+1000.0f, (y*microScale/megaScale)+1000.0f );
		
		return pixelHeight;
	}
	
	public float GetTerrainHeight( float x, float z )
	{
		/*
		// Map world coords to cell
		float lx = (x + mapsize * 0.5f)*(width-2.0f)/mapsize;
		float ly = (z + mapsize * 0.5f)*(height-2.0f)/mapsize;
		int cx = (int)lx;
		int cy = (int)ly;
		lx -= cx;		// Calc fractions across cell
		ly -= cy;
		
		float p1 = heightfield[cy*(width+2)+cx];
		float p2 = heightfield[cy*(width+2)+cx+1];
		float p3 = heightfield[(cy+1)*(width+2)+cx];
		float p4 = heightfield[(cy+1)*(width+2)+cx+1];
		
		float tx = p1*(1.0f-lx) + p2*lx;	// Top edge
		float bx = p3*(1.0f-lx) + p4*lx;	// Bottom edge
		
		return tx*(1.0f-ly) + bx*ly;
		*/
		return 0.0f;
	}
	
	void GenerateMesh()
	{
		Debug.Log("Starting mesh generation...");
		
		Random.seed = (int)(transform.position.x + 3.0f * transform.position.z);
		
		// Retrieve a mesh instance
		Mesh mesh = GetComponent<MeshFilter>().mesh;
			
		// Build vertices and UVs
		Vector2 uvScale 	= new Vector2(1.0f / (float)(width+1), 1.0f / (float)(height+1));
		float oneOverXCells = 1.0f / (float)(width-1);
		float oneOverZCells = 1.0f / (float)(height-1);
		
		// Pass one - fill in corner heights
		Debug.Log(" Pass 1 - fill in fractal points");
		
		int overlapDist = 8;
		
		for (int x=0; x<=width; x++)
		{
			float overlapWeight = 0.0f;
			if (x>width-overlapDist)
			{
				overlapWeight = (float)(x-width+overlapDist)/(float)(overlapDist-1);
			}
		
			for (int y=0; y<=height; y++)
			{
				float perlinX = -cylinderRadius*Mathf.PI*0.5f+(x*cylinderRadius*Mathf.PI*oneOverXCells);
				float perlinY = -cylinderLength*0.5f+(y*cylinderLength*oneOverZCells);
				float pixelHeight = GetFractal( perlinX, perlinY );
				if (overlapWeight>0.0f)
				{
					pixelHeight *= 1.0f-overlapWeight;
					pixelHeight += overlapWeight * GetFractal( -cylinderRadius*Mathf.PI*0.5f+((x-width+1)*cylinderRadius*Mathf.PI*oneOverXCells), perlinY );
				}
				heightfield[y*(width+2) + x] = pixelHeight;
			}
		}
		
		// Pass two - generate geometry
		// Walk flat grid and map height field onto a tapered cylinder
		// TODO: Biome variations?
		Debug.Log(" Pass 2 - generate geometry");
		
		for (int x=0; x<width; x++)	// Outer loop steps through pole-to-pole strips
		{	
			// Calc angle for this strip outside the inner loop as it's independent of heightfield
			float angle = 2.0f*Mathf.PI*x/(width-1.0f);
			float cosAngle = Mathf.Cos( angle );
			float sinAngle = Mathf.Sin( angle );
	
			for (int y=0; y<height; y++)
			{			
				//float p1 = heightfield[y*(width+2)+x];
				//float p2 = heightfield[y*(width+2)+x+1];
				//float p3 = heightfield[(y+1)*(width+2)+x];
				//float p4 = heightfield[(y+1)*(width+2)+x+1];
				
				Vector3 vertex;
				vertex.z = cylinderLength*y/(height-1.0f) - cylinderLength * 0.5f;
				
				float taper = Mathf.Cos( vertex.z*Mathf.PI/cylinderLength );
				float h = heightfield[y*(width+2)+x];
				float r = -cylinderRadius + h;
				r *= 0.2f + (0.8f * taper);
				
				vertex.x = r*cosAngle;
				vertex.y = r*sinAngle;
				
				vertices[y*width + x] = vertex;
				Vector2 temp_uv = new Vector2((float)x, (float)y);
				uv[y*width + x] = Vector2.Scale(temp_uv, uvScale);
			}
		}
		
		Color col = Color.white;
		for (int y=0; y<texSize; y++)
		{
			for (int x=0; x<texSize; x++)
			{
				float rand_unit = Random.value;
				rand_unit *= 0.1f;
				rand_unit += 0.9f;
				col.r = col.g = col.b = rand_unit;
				texture.SetPixel(x, y, col);
			}
		}
		
		// Pass 4 - build index buffer
		Debug.Log(" Pass 4 - build index buffer");
		
		texture.Apply();
		
		// Assign them to the mesh
		mesh.vertices = vertices;
		mesh.uv = uv;
	
		// Build triangle indices: 3 indices into vertex array for each triangle
		int index = 0;
		for (int y=0; y<height-1; y++)
		{
			for (int x=0; x<width-1; x++)
			{
				// For each grid cell output two triangles
				float p1 = heightfield[y*(width+2)+x];
				float p2 = heightfield[y*(width+2)+x+1];
				float p3 = heightfield[(y+1)*(width+2)+x];
				float p4 = heightfield[(y+1)*(width+2)+x+1];
				
				if (p1+p2+p3+p4 > 0.001f)	// Skip generating triangles at water level
				{
					triangles[index++] = (y     * width) + x;
					triangles[index++] = ((y+1) * width) + x;
					triangles[index++] = (y     * width) + x + 1;
			
					triangles[index++] = ((y+1) * width) + x;
					triangles[index++] = ((y+1) * width) + x + 1;
					triangles[index++] = (y     * width) + x + 1;
				}
			}
		}
		// And assign them to the mesh
		mesh.triangles = triangles;
			
		// Auto-calculate vertex normals from the mesh
		mesh.RecalculateNormals();
		
		MeshCollider collider = transform.GetComponent<MeshCollider>();
		collider.sharedMesh = emptyMesh;	// Flush cached physics-friendly version of old mesh
		collider.sharedMesh = mesh;
		
		// Pass 5 - populate surface features
		Debug.Log(" Pass 5 - populate surface features");
		
		for (int i=0; i<treeCount; i++)
		{
			Vector2 flatpos;
			flatpos.x = Random.Range( 0.0f, cylinderRadius*Mathf.PI );
			flatpos.y = Random.Range( 0.0f, cylinderLength );
			
			float h = GetTerrainHeight( flatpos.x, flatpos.y );
			
			// Calc angle for this strip outside the inner loop as it's independent of heightfield
			float angle = 2.0f*Mathf.PI*flatpos.x/(width-1.0f);
			float cosAngle = Mathf.Cos( angle );
			float sinAngle = Mathf.Sin( angle );
			float r = -cylinderRadius + h;
			Vector3 realpos;
			realpos.z = flatpos.y - cylinderLength * 0.5f;
			
			float taper = Mathf.Cos( realpos.z*Mathf.PI/cylinderLength );
			r *= 0.2f + (0.8f * taper);
			
			realpos.x = r*cosAngle;
			realpos.y = r*sinAngle;
			
			Instantiate( treePrefab1, realpos, Quaternion.identity );
		}		
		
		Debug.Log("Finished mesh generation.");
	}
	

	
	// Update is called once per frame
	void Update ()
	{

	}
}
