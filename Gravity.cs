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

public class Gravity : MonoBehaviour {

	protected float gravityForce;
	protected Vector3 targetDirection;

	//Adds a downwards directed force to the game object. If adjust rotation is true
	//the attracted objects up axis will point in up direction, in relation to the attracting object.
	//This is especially relevant if you have a spherical ground!
	public void Attract(Transform _attractedObject, bool _adjustRotation){
		gravityForce = _attractedObject.GetComponent<Rigidbody>().mass * -10.0f;

		targetDirection = (_attractedObject.position - transform.position).normalized;
		if (_adjustRotation == true){
			_attractedObject.rotation = Quaternion.FromToRotation(_attractedObject.up, targetDirection) * _attractedObject.rotation;
		}

		_attractedObject.GetComponent<Rigidbody>().AddForce(gravityForce * Vector3.up);
	}
}
