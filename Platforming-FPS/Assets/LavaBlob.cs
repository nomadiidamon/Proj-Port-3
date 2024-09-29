using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaBlob : MonoBehaviour
{
    public float damageRadius = 5f; // Radius of the AoE damage
    public int damageAmount = 10; // Damage to apply to each enemy
    public GameObject impactEffectPrefab; // Optional impact effect prefab

    private void OnTriggerEnter(Collider other)
    {
        // Check if the lava blob hits the ground or any valid target
        if (other.CompareTag("Ground")) // Adjust tag as needed
        {
            // Optional: Spawn an impact effect at the position
            if (impactEffectPrefab != null)
            {
                Instantiate(impactEffectPrefab, transform.position, Quaternion.identity);
            }

            // Apply AoE damage
            ApplyAreaOfEffectDamage();

            // Destroy the lava blob after impact
            Destroy(gameObject);
        }
    }

    private void ApplyAreaOfEffectDamage()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, damageRadius);
        Debug.Log($"Hit colliders count: {hitColliders.Length}");

        foreach (Collider collider in hitColliders)
        {
            // Only process the box collider (for example, if you know that only the feet have this component)
            if (collider is BoxCollider && collider.CompareTag("Enemy"))
            {
                Debug.Log($"Collider hit: {collider.name} - Tag: {collider.tag}");

                IDamage damageable = collider.GetComponent<IDamage>();
                if (damageable != null)
                {
                    damageable.takeDamage(damageAmount);
                    Debug.Log($"Damaged: {collider.name} for {damageAmount} damage");
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, damageRadius);   
    }
}
