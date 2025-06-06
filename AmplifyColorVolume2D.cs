using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
[AddComponentMenu("Image Effects/Amplify Color Volume 2D")]
public class AmplifyColorVolume2D : AmplifyColorVolumeBase
{
	private void OnTriggerEnter2D(Collider2D other)
	{
		AmplifyColorTriggerProxy2D component = other.GetComponent<AmplifyColorTriggerProxy2D>();
		if (component != null && component.OwnerEffect.UseVolumes && ((int)component.OwnerEffect.VolumeCollisionMask & (1 << base.gameObject.layer)) != 0)
		{
			component.OwnerEffect.EnterVolume(this);
		}
	}

	private void OnTriggerExit2D(Collider2D other)
	{
		AmplifyColorTriggerProxy2D component = other.GetComponent<AmplifyColorTriggerProxy2D>();
		if (component != null && component.OwnerEffect.UseVolumes && ((int)component.OwnerEffect.VolumeCollisionMask & (1 << base.gameObject.layer)) != 0)
		{
			component.OwnerEffect.ExitVolume(this);
		}
	}
}
