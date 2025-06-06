using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
[AddComponentMenu("Image Effects/Amplify Color Volume")]
public class AmplifyColorVolume : AmplifyColorVolumeBase
{
	private void OnTriggerEnter(Collider other)
	{
		AmplifyColorTriggerProxy component = other.GetComponent<AmplifyColorTriggerProxy>();
		if (component != null && component.OwnerEffect.UseVolumes && ((int)component.OwnerEffect.VolumeCollisionMask & (1 << base.gameObject.layer)) != 0)
		{
			component.OwnerEffect.EnterVolume(this);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		AmplifyColorTriggerProxy component = other.GetComponent<AmplifyColorTriggerProxy>();
		if (component != null && component.OwnerEffect.UseVolumes && ((int)component.OwnerEffect.VolumeCollisionMask & (1 << base.gameObject.layer)) != 0)
		{
			component.OwnerEffect.ExitVolume(this);
		}
	}
}
