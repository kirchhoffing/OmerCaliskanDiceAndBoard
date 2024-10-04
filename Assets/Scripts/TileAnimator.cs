using System.Collections;
using UnityEngine;

public class TileAnimator : MonoBehaviour
{
    public static TileAnimator instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AnimateTile(Transform tile)
    {
        StartCoroutine(BounceTile(tile));
    }

    private IEnumerator BounceTile(Transform tile)
    {
        Vector3 originalPosition = tile.position;
        Vector3 raisedPosition = originalPosition + new Vector3(0, 0.5f, 0);

        float bounceTime = 0;
        float bounceDuration = 0.3f;
        
        while (bounceTime < bounceDuration)
        {
            bounceTime += Time.deltaTime;
            tile.position = Vector3.Lerp(originalPosition, raisedPosition, bounceTime / bounceDuration);
            yield return null;
        }

        yield return new WaitForSeconds(0.1f);

        bounceTime = 0;

        while (bounceTime < bounceDuration)
        {
            bounceTime += Time.deltaTime;
            tile.position = Vector3.Lerp(raisedPosition, originalPosition, bounceTime / bounceDuration);
            yield return null;
        }

        tile.position = originalPosition;
    }
}