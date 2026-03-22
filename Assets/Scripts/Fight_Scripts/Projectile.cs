using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour
{
    public IEnumerator FlyToTarget(Vector3 startPos, Vector3 targetPos, Color projColor, float speed)
    {
        // 1. Nak³adamy  filtr koloru na strza³ê
        SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
        if (sr != null) sr.color = projColor;

        // 2. Ustawiamy j¹ na klatce piersiowej £ucznika
        transform.position = startPos;

        // 3. OBRÓT
        Vector3 dir = targetPos - startPos;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        // 4. LOT
        while (Vector3.Distance(transform.position, targetPos) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
            yield return null; // Czekamy do nastêpnej klatki
        }

        // 5. TRAFIENIE: Strza³a dolecia³a, Usuwamy j¹ z ekranu i pozwalamy bitwie trwaæ dalej.
        Destroy(gameObject);
    }
}