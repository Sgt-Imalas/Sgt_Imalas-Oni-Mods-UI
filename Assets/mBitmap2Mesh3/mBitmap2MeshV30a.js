// Bitmap2Mesh v3.0 - 09.02.2012 - mgear - http://unitycoder.com/blog
// ---
// Donations are welcome, especially if this is used for commercial projects.. see donation link the blog : )
// ---

// USAGE
// Attach this script to gameObject with meshFilter and bitmap2mesh_testmaterial
// assign source texture, set extrude depth, assign same texture to the material, press play.. "myObject3D" should be created.

// BITMAPS
// Good texture import settings: 
// texture should be power of 2 otherwise gets blurred..(?)
// Filter mode: Point, Wrap mode: Repeat
// Texture type: Advanced, Read/Write: Enabled [x], MaxSize: ##, Texture Format: RGBA 32bit

//import System.IO; // for saving text file

public var SourceTexture:Texture2D; // texture to be scanned

// scaling removed, doesnt work right now.. you can just scale the created gameObject itself..
private var Pixel_vs_Unit_Scale:float = 1;  // 1 = 1 units (m), must be 1 for now.

public var extrudeDepth:float=1; // in unity units (1 = 1 unit (m)
private var scanpoint:Vector2;
private var gotStart:boolean=false;
private var maxsteps:int=512; // max steps for searching the outline, its NOT the max vertice amount

// extrude part of the code is from Unity Procedural Examples:
// Generates an extrusion trail from the attached mesh
// Uses the MeshExtrusion algorithm in MeshExtrusion.cs to generate and preprocess the mesh.
public var invertFaces = false;
private var srcMesh : Mesh;
private var precomputedEdges : mMeshExtrusion2.Edge[];
private var sections = new Array();
class ExtrudedTrailSectionBM3
{
	var point : Vector3;
	var matrix : Matrix4x4;
	var time : float;
}

// init stuff
function Start ()
{

	// this loop will scan for 1st pixel that is not transparent
	for (var y:int=0;y<SourceTexture.width;y++)
	{
		for (var x:int=0;x<SourceTexture.height;x++)
		{
			if (SourceTexture.GetPixel(x, y).a>0)
			{
				if (!gotStart) // founded a pixel which is not transparent/empty
				{
						scanpoint = Vector2(x,y); // take this location
						gotStart = true;
						y=SourceTexture.height;
						x=SourceTexture.width;
						break;
				}
			}
		}
	}
	
	// init variables, used for following the bitmap outline
	var l:int = 0; // while-loop counter
	var dir:int=1; // 0 = up, 1 = right, 2 = down, 3=left
	var olddir:int=-1; // previous direction ^
	
	// move vector: if its (1,0) it means, x+1 (move right)
	var move:Vector2=Vector2(0,0);
	var startpoint:Vector2 = scanpoint; // copy our starting pixel location
	
	var current:boolean=false; // does our current scan location has pixel
	var up:boolean=false;
	var right:boolean=false;
	var down:boolean=false;
	var left:boolean=false;
	var moved:boolean=false;
	var count:int=0; // vertex counter
	
	var polys = new Array(); // array to collect vertex locations
	
	// TODO: still need to cleanup the scanning part..recursive function??
	// this while loop will follow the bitmap image edges, (and collect vertex positions)
	// until we hit startpoint or hit iteration maxsteps
	while (l < maxsteps)
	{
		// lets go, check neighbour cell values (if any of them has color)
		current=CheckPixelAlpha(scanpoint.x,scanpoint.y);
		
		up=CheckPixelAlpha(scanpoint.x,scanpoint.y+1);
		right=CheckPixelAlpha(scanpoint.x+1,scanpoint.y);
		down=CheckPixelAlpha(scanpoint.x,scanpoint.y-1);
		left=CheckPixelAlpha(scanpoint.x-1,scanpoint.y);

		moved=false;
		
		// TODO: optimize/cleanup these parts : )
		if (!moved)
		{
			if (dir==0) // what is our direction? 0=about to go up
			{
				moved=true;
				if (left) // founded pixel from left direction
				{
					dir=3; // so we take direction as 3 = left
					move=Vector2(-1,0); // and move vector is -1 = left
				}else{
					if (up) // founded pixel from 
					{
						dir=0;
						move=Vector2(0,1);
					}else{
						if (right) // founded pixel from 
						{
							dir=1;
							move=Vector2(1,0);
						}else{
							if (down) // founded pixel from 
							{
								dir=2;
								move=Vector2(0,-1);
							}else{
								// no pixels, error??
								print ("Error1."+l);
							} // if left
						} // if down
					} // if right
				} // if up
			}
		}

		if (!moved)
		{
			if (dir==1) // about to go right
			{
				moved=true;
				if (up) // founded pixel from up
				{
					dir=0;
					move=Vector2(0,1);
				}else{
					if (right) // founded pixel from right
					{
						dir=1;
						move=Vector2(1,0);
					}else{
						if (down) // founded pixel from down
						{
							dir=2;
							move=Vector2(0,-1);
						}else{
							if (left) // founded pixel from left
							{
								dir=3;
								move=Vector2(-1,0);
							}else{
								// no pixels, error
								print ("Error2."+l);
							} // if left
						} // if down
					} // if right
				} // if up
			}
		}

		if (!moved)
		{
			if (dir==2) // about to go down
			{
				moved=true;
				if (right) // founded pixel from 
				{
					dir=1;
					move=Vector2(1,0);
				}else{
					if (down) // founded pixel from 
					{
						dir=2;
						move=Vector2(0,-1);
					}else{
						if (left) // founded pixel from 
						{
							dir=3;
							move=Vector2(-1,0);
						}else{
							if (up) // founded pixel from 
							{
								dir=0;
								move=Vector2(0,1);
							}else{
								// no pixels, error
								print ("Error3."+l);
							} // if left
						} // if down
					} // if right
				} // if up
			}
		}

		if (!moved)
		{
			if (dir==3) // about to go left
			{
				moved=true;
				if (down) // founded pixel from 
				{
					dir=2;
					move=Vector2(0,-1);
				}else{
					if (left) // founded pixel from 
					{
						dir=3;
						move=Vector2(-1,0);
					}else{
						if (up) // founded pixel from 
						{
							dir=0;
							move=Vector2(0,1);
						}else{
							if (right) // founded pixel from 
							{
								dir=1;
								move=Vector2(1,0);
							}else{
								// no pixels, error
								print ("Error4."+l);
							} // if left
						} // if down
					} // if right
				} // if up
			}
		}

		// current position has pixel? add point here
		if (current && olddir!=dir)
		{
			// 0 = up, 1 = right, 2 = down, 3=left
		
			var padding:Vector2=Vector2(0,0);
		
			// move point to pixel border, based on what direction we are coming
			if (olddir==-1)
			{
				if (dir==0) padding=Vector2(-0.5,-0.5); // previous step, we went up, so border is ..
				if (dir==1) padding=Vector2(0.5,-0.5);
				if (dir==2) padding=Vector2(0.5,0.5);
				if (dir==3) padding=Vector2(-0.5,0.5);
			}
			if (olddir==0)
			{
//				if (dir==0) padding=Vector2(-0.5,-0.5); // previous step, we went up, so border is ..
				if (dir==1) padding=Vector2(-0.5,0.5); // top left
//				if (dir==2) padding=Vector2(0.5,0.5);
				if (dir==3) padding=Vector2(-0.5,-0.5); // bottom left
			}

			if (olddir==1)
			{
				if (dir==0) padding=Vector2(-0.5,0.5);
//				if (dir==1) padding=Vector2(-0.5,0.5);
				if (dir==2) padding=Vector2(0.5,0.5); // top right
				if (dir==3) padding=Vector2(0.5,0); // middle right // TODO: fix single pixel problem.. needs extra vertice?
			}
			
			if (olddir==2)
			{
				if (dir==0) padding=Vector2(0,-0.5); // middle bottom // TODO: fix single pixel problem.. needs extra vertice?
				if (dir==1) padding=Vector2(0.5,0.5); // top right
//				if (dir==2) padding=Vector2(0.5,0.5);
				if (dir==3) padding=Vector2(0.5,-0.5);
			}

			if (olddir==3)
			{
				if (dir==0) padding=Vector2(-0.5,-0.5);
				if (dir==1) padding=Vector2(-0.5,0); // middle left // TODO: fix single pixel problem.. needs extra vertice?
				if (dir==2) padding=Vector2(0.5,-0.5);
//				if (dir==3) padding=Vector2(0.5,-0.5);
			}


//			polys.Push(((scanpoint+padding)-startpoint)*Pixel_vs_Unit_Scale);
//			polys.Push(((scanpoint+padding)-startpoint)*Pixel_vs_Unit_Scale);
//			polys.Push(((scanpoint+padding)-startpoint)*Pixel_vs_Unit_Scale);
			// 0.5, offset padding back so that pivot is 0,0 ?!?
			polys.Push((scanpoint+padding)+Vector2(0.5,0.5)*Pixel_vs_Unit_Scale);
//			print ("createVert:"+count+" : "+polys[polys.length-1]);
			//var dummen = transform.InverseTransformPoint(scanpoint);
//			polys.Push(((Vector2(dummen.x,dummen.y)+padding)-startpoint)*Pixel_vs_Unit_Scale);
//			polys.Push(scanpoint);
			count++;
			olddir=dir;
			
//			print (whatshappeninghere);
		}
		
		scanpoint+=move;
	
		if (scanpoint==startpoint) 
		{
			print ("we succesfully arrived to the startpoint. Step:"+l);
			break;
		}
		l++;
	} 
	
	// check if we hit max scan steps, that means, we didnt reach our start point..(something will be missing)
	if (l==maxsteps) print ("problems..we hit maxsteps!!:"+maxsteps);
	
	// Copy the js array into a builtin array
	var poly:Vector2[] = polys.ToBuiltin(Vector2);
	
	// TEST
//	System.Array.Reverse(poly);
	
	// scanning done
	print ("done. Total vertices:"+poly.length);

	// actual work starts

	// triangulate 2D polygon
	var triangulator = Triangulator(poly);
	var tris = triangulator.Triangulate();
	
	var vertices:Vector3[] = new Vector3[poly.Length];


	// build vertex list
	for(var i:int=0;i<poly.Length;i++)
	{
		vertices[i].x = poly[i].x; // why minus??
		vertices[i].y = poly[i].y;
		vertices[i].z = 0;
	}


	// build custom UV (TODO: works for front/back, not for the sides..(sides UV is coming from extruder??)
	var uvs : Vector2[] = new Vector2[vertices.Length];
	// get bounding box
	var uvminx:float=999;
	var uvminy:float=999;
	var uvmaxx:float=-999;
	var uvmaxy:float=-999;
	
	// get max values
	for (i = 0 ; i < vertices.Length; i++)
	{
		if (vertices[i].x<uvminx) uvminx = vertices[i].x;
		if (vertices[i].y<uvminy) uvminy = vertices[i].y;
		if (vertices[i].x>uvmaxx) uvmaxx = vertices[i].x;
		if (vertices[i].y>uvmaxy) uvmaxy = vertices[i].y;
//			print (vertices[i].y);
	}
//	print ("uvmin xy="+uvminx+","+uvminy+" uvmax xy="+uvmaxy+","+uvmaxy);
	for (i = 0 ; i < vertices.Length; i++)
	{
		// added mirrorX UV map: "1-"
//		uvs[i] = Vector2(remap(1-vertices[i].x,uvminx,uvmaxx,0,1), remap(vertices[i].y,uvminy,uvmaxy,0,1));
//		uvs[i] = Vector2(remap(vertices[i].x,uvminx,uvmaxx,0,1), remap(vertices[i].y,uvminy,uvmaxy,0,1));
//		uvs[i] = Vector2 (vertices[i].x, vertices[i].y);
//		uvs[i] = (vertices[i]+Vector3(3,1,0))/17;

	// x,y is <0, because pivot is not in 0,0
//		uvs[i] = (vertices[i]+Vector3(3,1,0))/17;
		uvs[i] = vertices[i]/SourceTexture.width; // strange uv solution..works for now (?)
	}


	// create new object for this model (2D polygon should stay with the generator obj.)
	var myobject : GameObject;
	myobject = new GameObject ("myObject3D");
	myobject.AddComponent("MeshRenderer");
	var filter:MeshFilter = myobject.AddComponent("MeshFilter");
	
	var m:Mesh = new Mesh();
	GetComponent(MeshFilter).mesh = m;
	
	// 2D mesh
	m.vertices = vertices;
	m.uv = uvs;
	m.triangles = tris;
	m.RecalculateNormals(); // no need?
	m.RecalculateBounds(); // no need?
//	m.Optimize();

	// extrude stuff -- start
	srcMesh = GetComponent(MeshFilter).mesh;
	precomputedEdges = mMeshExtrusion2.BuildManifoldEdges(srcMesh);

	var position = transform.position;
	var now = Time.time;

		var section = ExtrudedTrailSectionBM3 ();
		section.point = position;
		section.matrix = transform.localToWorldMatrix;
		section.time = now;
		// using 2 sections
		sections.Unshift(section); // back
		sections.Unshift(section); // front
//		sections.Unshift(section); 

	var worldToLocal = transform.worldToLocalMatrix;
	var finalSections = new Matrix4x4[sections.length];

	finalSections[0] = Matrix4x4.identity;
	finalSections[1] = worldToLocal * Matrix4x4.TRS(transform.position+Vector3.forward*extrudeDepth, Quaternion.identity, Vector3.one);	
//	finalSections[2] = worldToLocal * Matrix4x4.TRS(transform.position+Vector3.forward*extrudeDepth, Quaternion.identity, Vector3.one);	
//	finalSections[2] = worldToLocal * Matrix4x4.TRS(transform.position+Vector3.forward*(extrudeDepth*2), Quaternion.identity, Vector3(1,1,1));	

	// Rebuild the extrusion mesh	
	mMeshExtrusion2.ExtrudeMesh (srcMesh, myobject.GetComponent(MeshFilter).mesh, finalSections, precomputedEdges, invertFaces);
	// extrude stuff -- end
	// needed ?
	//myobject.GetComponent(MeshFilter).mesh.Optimize();
	
	// assign material, currently as an instance material.. ? (TODO: need to create new material..since many objects can have the same? or use texture atlasing??)
	myobject.renderer.material = renderer.material;
	
	// TESTING: add collider, add rb to the object so it falls down
//	myobject.AddComponent(MeshCollider);
//	myobject.AddComponent(Rigidbody);
/*
	// re-fix up maps to planar? (those that are not back or front faces?)
	var xvertices : Vector3[] = myobject.GetComponent(MeshFilter).mesh.vertices;
	var xolduvs : Vector2[] = myobject.GetComponent(MeshFilter).mesh.uv;
	var xuvs : Vector2[] = new Vector2[xvertices.Length];
	var xnormals : Vector3[] = myobject.GetComponent(MeshFilter).mesh.normals;
	for (var xi = 0 ; xi < xuvs.Length; xi++)
	{
		if (xnormals[xi].x==1)
		{
//			xuvs[xi] = Vector2 (xvertices[xi].x, xvertices[xi].y);
//			xuvs[xi] = Vector2(remap(xvertices[xi].x,uvminx,uvmaxx,0,1), 1);
//			xuvs[xi] = Vector2(remap(xvertices[xi].y,uvminy,uvmaxy,0,1), remap(xvertices[xi].x,uvminx,uvmaxx,0,1));
		}	
		
		if (xnormals[xi].x==0) // front / back, original uv
		{
//			print (xolduvs.Length);
			xuvs[xi] = xolduvs[xi];
		}	
		
	}*/
	
	// build custom UV (TODO: works for front/back, not for the sides..(sides UV is coming from extruder??)
	var xvertices : Vector3[] = myobject.GetComponent(MeshFilter).mesh.vertices;
//	var xuvs : Vector2[] = new Vector2[xvertices.Length];
	var xuvs : Vector2[] = myobject.GetComponent(MeshFilter).mesh.uv;
//	var xolduvs : Vector2[] = myobject.GetComponent(MeshFilter).mesh.uv;
	var xnormals : Vector3[] = myobject.GetComponent(MeshFilter).mesh.normals;
	// get bounding box
	uvminx=9999;
	uvminy=9999;
	uvminz=9999;
	uvmaxx=-9999;
	uvmaxy=-9999;
	uvmaxz=-9999;
	
	// get max values
	for (i = 0 ; i < vertices.Length; i++)
	{
		if (xvertices[i].x<uvminx) uvminx = xvertices[i].x;
		if (xvertices[i].y<uvminy) uvminy = xvertices[i].y;
		if (xvertices[i].z<uvminz) uvminz = xvertices[i].z;
		if (xvertices[i].x>uvmaxx) uvmaxx = xvertices[i].x;
		if (xvertices[i].y>uvmaxy) uvmaxy = xvertices[i].y;
		if (xvertices[i].z>uvmaxz) uvmaxz = xvertices[i].z;
//			print (vertices[i].y);
	}
//	print ("uvmin xy="+uvminx+","+uvminy+" uvmax xy="+uvmaxy+","+uvmaxy);
	for (i = 0 ; i < xvertices.Length; i++)
	{
		// works with mirrored?
		if (xnormals[i].x==1) // right side
		{
//			xuvs[i] = Vector2(1-remap(xvertices[i].x,uvminx,uvmaxx,0,1), remap(xvertices[i].y,uvminy,uvmaxy,0,1));
//			xuvs[i] = Vector2(remap(xvertices[i].x,uvminx,uvmaxx,0,1), remap(xvertices[i].y,uvminy,uvmaxy,0,1));
//			xuvs[i] = Vector2(xvertices[i].x,xvertices[i].y);
			xuvs[i] = Vector2(xvertices[i].x-0.00001,xvertices[i].y)/SourceTexture.width; // 16
///			xuvs[i].x = 0.9999;
//			print ("uvfix:"+i);
		}
		
		if (xnormals[i].y==1) // top
		{
//			xuvs[i] = Vector2(remap(xvertices[i].x,uvminx,uvmaxx,0,1), remap(xvertices[i].y,uvminy,uvmaxy,0,1));
//			xuvs[i] = Vector2(remap(xvertices[i].x,uvminx,uvmaxx,0,1), remap(xvertices[i].y,uvminy,uvmaxy,0,1));
			xuvs[i] = Vector2(xvertices[i].x,xvertices[i].y-0.00001)/SourceTexture.width; 
//			xuvs[i] = Vector2(0,0);
//			xuvs[i].y = 0.9999;
//			print ("fixed:"+xuvs[i].x+","+xuvs[i].y);
//			xuvs[i].y -=0.001;
//			print ("uvfix:"+i);
		}

		if (xnormals[i].y==-1) // bottom
		{
//			xuvs[i] = Vector2(remap(xvertices[i].x,uvminx,uvmaxx,0,1), remap(xvertices[i].y,uvminy,uvmaxy,0,1));
//			xuvs[i].y = 0.9999;
			xuvs[i] = Vector2(xvertices[i].x,xvertices[i].y+0.00001)/SourceTexture.width; 
		//	print ("uvfix:"+i);
		}
		
		if (xnormals[i].x==-1) // left side
		{
//			xuvs[i] = Vector2(remap(xvertices[i].x,uvminx,uvmaxx,0,1), remap(xvertices[i].y,uvminy,uvmaxy,0,1));
			// something needs to be 0,0..?
//			xuvs[i] = Vector2(remap(xvertices[i].z,uvminz,uvmaxz,0,1), remap(xvertices[i].y,uvminy,uvmaxy,0,1));
			xuvs[i] = Vector2(xvertices[i].x+0.00001,xvertices[i].y)/SourceTexture.width; 
//			if (xuvs[i].x<0) xuvs[i].x = 0;
//			if (xuvs[i].x>1) xuvs[i].x = 1;
//			xuvs[i] = xolduvs[i];
		}
		
		if ((xnormals[i].x==0) && (xnormals[i].y==0)) // front / back, original uv
		{
//			xuvs[i] = xolduvs[i];
		}			
		//	xuvs[i] = xolduvs[i];
	}	
		myobject.GetComponent(MeshFilter).mesh.uv = xuvs;
		
	//DumpInfo(myobject.GetComponent(MeshFilter).mesh);




}

//@script RequireComponent (MeshFilter)




// custom function: remap value range to another range
function remap(value:float, from1:float, to1:float, from2:float, to2:float) 
{
	return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
}

// custom function: checks pixel alpha value from texture(global), returns true if > 0
function CheckPixelAlpha(x:int,y:int) 
{
	var pa:boolean=false;
	
	// check limits
	if (x<0) return false;
	if (x>SourceTexture.width) return false;
	if (y>SourceTexture.height) return false;
	if (y<0) return false;	
	
	// check pixel alpha, could use other colors or methods also..
	if (SourceTexture.GetPixel(x, y).a>0) {pa=true;}else{pa=false;}
	return pa;
}

/*
// TESTING: dump mesh info to file
function DumpInfo (mesh:Mesh) 
{
//	var mesh : Mesh = GetComponent(MeshFilter).mesh;
	
	var vertices : Vector3[] = mesh.vertices;
	var tris = new Array(mesh.triangles);
	var normals : Vector3[] = mesh.normals;
	var UVs : Vector2[] = mesh.uv;
	
	    // Create an instance of StreamWriter to write text to a file.
    sw = new StreamWriter("TestFile.txt");
    // Add some text to the file.
	
	sw.WriteLine("Vertices:"+vertices.length);
	for (var i:int;i<vertices.length;i++)
	{
		sw.WriteLine("vert#"+(i+1)+":"+vertices[i]);
	}
	sw.WriteLine("");
	sw.WriteLine("Faces:"+tris.length);
	for (i=0;i<tris.length;i++)
	{
		sw.Write("face#"+(i+1)+":"+tris[i]);
		sw.Write(" : "+vertices[tris[i]]);
		sw.WriteLine("");
	}
	sw.WriteLine("");
	sw.WriteLine("UVs:"+UVs.length);
	for (i=0;i<UVs.length;i++)
	{
		sw.WriteLine("UV#"+(i+1)+":"+UVs[i].x+","+UVs[i].y);
	}
	sw.WriteLine("");
	sw.WriteLine("NORMALs:"+normals.length);
	for (i=0;i<normals.length;i++)
	{
		sw.WriteLine("normal#"+(i+1)+":"+normals[i]);
	}

    sw.Close();
}
*/