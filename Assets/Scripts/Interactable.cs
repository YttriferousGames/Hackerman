using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider), typeof(Renderer))]
public class Interactable : MonoBehaviour {
    public delegate void InteractEvent(Interactable sender);
    public event InteractEvent OnInteract;
    private Collider col;
    public Renderer outlineRenderer = null;
    private bool was_hovering = false;
    [HideInInspector]
    public bool hovering = false;

    private void Awake() {
        col = GetComponent<Collider>();
        if (outlineRenderer == null)
            outlineRenderer = GetComponent<Renderer>();
    }

    public void Interact() {
        hovering = true;
        OnInteract(this);
    }

    // Update is called once per frame
    private void Update() {
        if (hovering && !was_hovering) {
            outlineRenderer.material.SetColor("_OutlineColor", Color.green);
        } else if (!hovering && was_hovering) {
            outlineRenderer.material.SetColor("_OutlineColor", Color.black);
        }
        was_hovering = hovering;
        hovering = false;
    }
}
