using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Ce syst�me de patrouille est bas� sur la m�thode des waypoints
// ce syst�me consitet � d�finir des points de passage pour l'ennemi
// l'ennemi se d�place de point en point (aller-retour)
public class EnemyPatrol : MonoBehaviour
{
    public float speed;
    public Transform[] waypoints;

    [SerializeField]
    private GameObject player;

    [SerializeField]
    private SpriteRenderer spriteRenderer;

    //Variable pour stocker le point de passage vers lequel l'ennemie ce dirige
    private Transform target;
    //Variable pour stocker l'index du point de passage vers lequel l'ennemie ce dirige (Le m�me que celui de la variable target)
    private int destPoint = 0;
    //Variable pour stocker le weakSpot de l'ennemie et le r�utiliser pour v�rifier si le joueur est en collision avec
    private BoxCollider2D weakPoint;

    // Start is called before the first frame update
    void Start()
    {
        //On initialise le point de passage de l'ennemie � son premier point de passage
        target = waypoints[0];

        //On ajoute un weakSpot � l'ennemie pour pouvoir le tuer
        CreateBoxCollider2D(new Vector2(0f, 0.1f), new Vector2(0.1f, 0.02f));
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 dir = target.position - transform.position;
        //On normalise le vecteur (sa longueur devient 1)
        transform.Translate(dir.normalized * speed * Time.deltaTime, Space.World);

        //Si l'ennemie est proche du point de passage (le superpos)
        if (Vector3.Distance(transform.position, target.position) < 0.3f)
        {
            //On parcours notre liste de waypoints modulo la taille de la liste pour �viter les d�bordements
            destPoint = (destPoint + 1) % waypoints.Length;
            target = waypoints[destPoint];
        }

        
        if(weakPoint.IsTouching(player.GetComponent<CircleCollider2D>()))
        {
            player.GetComponent<Rigidbody2D>().AddForce(new Vector2(0f, 160.0f)); ;
            Destroy(transform.parent.gameObject);
        }

        Flip(dir.x);
    }

    void CreateBoxCollider2D(Vector2 offset, Vector2 size)
    {
        BoxCollider2D boxCollider2D = gameObject.AddComponent<BoxCollider2D>();
        boxCollider2D.offset = offset;
        boxCollider2D.size = size;
        weakPoint = boxCollider2D;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.transform.GetComponent<PlayerHealth>();
            playerHealth.TakeDamage();
        }
    }

    void Flip(float speed)
    {
        if (speed < 0.1f)
        {
            spriteRenderer.flipX = true;
        }
        else if (speed > -0.1f)
        {
            spriteRenderer.flipX = false;
        }
    }
}
