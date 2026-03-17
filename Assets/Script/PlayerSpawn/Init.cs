using UnityEngine;

public class Init : MonoBehaviour
{
    void Start()
    {
        GameObject prefab = CharacterSelect.selectedCharacter;

        if (prefab == null)
        {
            Debug.LogError("Ch?a ch?n nhân v?t!");
            return;
        }

        Vector3 spawnPos = transform.position;
        spawnPos.z = 0f;

        GameObject player = Instantiate(prefab, spawnPos, Quaternion.identity);

        // ??m b?o không b? n?m d??i map
        SpriteRenderer sr = player.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.sortingLayerName = "Player";
            sr.sortingOrder = 0;
        }

        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.freezeRotation = true;
        }
    }
}