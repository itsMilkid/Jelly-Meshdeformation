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

public class MouseInput : MonoBehaviour {

	[Header("Input Settings:")]
	[SerializeField] protected float pressureForce;
	[SerializeField] protected float forceOffset;

	protected RaycastHit hitInfo;
	protected Ray ray;

	protected Vector3 inputPoint;
	protected JellyPhysics jellyBody;

	private void Update(){
		CheckForMouseInput();
	}

	//Transforms a ray from the mouse position, if the ray hits a jelly
	//object it will determine the hit put in consideration of the set
	//force offset and calls the hit objects JellyPhysics AddForce function 
	//to apply the pressure to the jelly mesh object
	private void CheckForMouseInput(){
		if(Input.GetMouseButton(0)){
			ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		}

		if(Physics.Raycast(ray,out hitInfo)){
			jellyBody = hitInfo.collider.GetComponent<JellyPhysics>();
			if(jellyBody != null){
				inputPoint = hitInfo.point;
				inputPoint += hitInfo.normal * forceOffset;
				jellyBody.AddForce(inputPoint,pressureForce);
			}
		}
	}
}
