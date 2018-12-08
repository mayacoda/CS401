using UnityEngine;
using System.Collections;

public class DestroyOnCollision : MonoBehaviour {

	private void OnCollisionEnter(Collision col)
    {
		Destroy (col.gameObject);
	}
}
