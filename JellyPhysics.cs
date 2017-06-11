#region Copyright
//MIT License
//Copyright (c) 2017 , Milkid - Kristin Stock 

//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//SOFTWARE.

#endregion

using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(Rigidbody))]
public class JellyPhysics : MonoBehaviour {

	[Header("Jelly Settings")]
	[SerializeField] protected bool enableGravity;
	[SerializeField] protected bool allowRotation;
	[SerializeField][Range(1.0f,50.0f)] protected float stiffness;
	[SerializeField][Range(0.1f,20.0f)] protected float attenuation;
	[SerializeField][Range(0.0f,1.0f)] protected float forceOffset;

	protected Mesh mesh;
	protected Rigidbody rigidBody;
	protected Gravity gravity;

	protected float initialForce;

	protected float scaleUniformation;

	protected Vector3[] initialMeshVertices;
	protected Vector3[] displacedMeshVertices;
	protected Vector3[] vertexVelocities;

	protected Vector3 inputPointToVertex;
	protected Vector3 vertexVelocity;
	protected Vector3 vertexDisplacement;

	protected Vector3 currentContactPoint;

	protected float stationaryForce;
	protected float stationaryVelocity;

	#region Initialization

	private void Start(){
		if(enableGravity == true){
			gravity   = GameObject.FindGameObjectWithTag("Ground").GetComponent<Gravity>();
		}
		
		rigidBody = GetComponent<Rigidbody>();
		rigidBody.useGravity  = false;
		rigidBody.constraints = RigidbodyConstraints.FreezeRotation;
		initialForce = rigidBody.mass;
		mesh = GetComponent<MeshFilter>().mesh;
		initialMeshVertices = mesh.vertices;

		displacedMeshVertices = new Vector3[initialMeshVertices.Length];
		for(int i = 0; i < initialMeshVertices.Length; i++){
			displacedMeshVertices[i] = initialMeshVertices[i];
		}
		vertexVelocities = new Vector3[initialMeshVertices.Length];
	}

	#endregion

	//Sets scaleUniformation to mesh's transform.localScales x-Axis value to secure scale.
	//Loops through mesh's vertices and updates mesh and recalculate it's normals.
	private void Update(){
		scaleUniformation = transform.localScale.x;
		for(int i = 0; i < displacedMeshVertices.Length; i++){
			UpdateVertices(i);
		}
		mesh.vertices = displacedMeshVertices;
		mesh.RecalculateNormals();
	}

	//If gravity is enable, this transform will be attracted to the ground object in this scene.
	private void FixedUpdate(){
		if(enableGravity == true){
			gravity.Attract(transform,allowRotation);
		}
	}

	//Input point will be transformed into it's inverse transformpoint, loops through displacesMeshVertices
	//array to add force to each necessary vertex.
	public void AddForce(Vector3 _inputPoint, float _inputForce){
		_inputPoint = transform.InverseTransformPoint(_inputPoint);

		for(int i = 0; i < displacedMeshVertices.Length; i++){
			AddVelocityToVertices(i,_inputPoint,_inputForce);
		}
	}

	// Transforms a contact point into its corresponding vertex, calculates the force onto this vertex and 
	// transforms it into it's velocity. Updates vertexVelocities array for said vertex.
	private void AddVelocityToVertices(int _vertexIndex,Vector3 _inputPoint, float _inputForce){
		inputPointToVertex = displacedMeshVertices[_vertexIndex] - _inputPoint;
		inputPointToVertex *= scaleUniformation;

		stationaryForce = _inputForce/(1.0f + inputPointToVertex.sqrMagnitude);
		stationaryVelocity = stationaryForce * Time.deltaTime;
		vertexVelocities[_vertexIndex] += inputPointToVertex.normalized * stationaryVelocity;
	}

	//Calculates vertex displacement. Makes sure vertexVelocity will get dampened over time to ensure 
	//reversing to intial mesh's shape.
	private void UpdateVertices(int _vertexIndex){
		vertexDisplacement = displacedMeshVertices[_vertexIndex] - initialMeshVertices[_vertexIndex];
		vertexDisplacement *= scaleUniformation;
		
		vertexVelocity = vertexVelocities[_vertexIndex];
		vertexVelocity -= vertexDisplacement * stiffness * Time.deltaTime;
		vertexVelocity *= 1.0f - attenuation * Time.deltaTime;

		vertexVelocities[_vertexIndex] = vertexVelocity;
		displacedMeshVertices[_vertexIndex] += vertexVelocity * (Time.deltaTime/scaleUniformation);	
	}

	//Calling mesh deformation upon collision stay events.
	private void OnCollisionStay(Collision _collision){
		if(_collision.gameObject != gameObject){
			if(_collision.contacts.Length > 0){
				for(int i = 0; i < _collision.contacts.Length; i++){
					currentContactPoint = _collision.contacts[i].point;
					currentContactPoint += _collision.contacts[i].normal * forceOffset;
					AddForce(currentContactPoint,initialForce);
				}
			}
		}
	}
}
