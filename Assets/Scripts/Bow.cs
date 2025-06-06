﻿using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine;

public class Bow : XRGrabInteractable
{ // still not working
    private bool isGrabbed = false;

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        this.GetComponent<AudioSource>().Play();
        if (!isGrabbed)
        {
            base.OnSelectEntered(args);
            isGrabbed = true;
            //Debug.Log("Bow grabbed"); (works)
        }
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
        isGrabbed = false;
        //Debug.Log("Bow released");
    }
}
