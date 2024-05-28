using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Ce syst�me de patrouille est bas� sur la m�thode des waypoints
// ce syst�me consitet � d�finir des points de passage pour l'ennemi
// l'ennemi se d�place de point en point (aller-retour)
public class EnemyPatrol : MonoBehaviour
{
    private Animator anim; // R�f�rence � l'Animator
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
        //On r�cup�re l'Animator de l'ennemi
        anim = GetComponent<Animator>();

        //On initialise le point de passage de l'ennemie � son premier point de passage
        target = waypoints[0];

        player = GameObject.FindGameObjectWithTag("Player");

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

        if (player == null)
        {
            return;
        }

        if(weakPoint.IsTouching(player.GetComponent<CircleCollider2D>()))
        {
            //Permet de normaliser la v�locit� du joueur pour �viter d'avoir une inconsistance sur la hauteur de saut
            player.GetComponent<Rigidbody2D>().velocity = new Vector2(player.GetComponent<Rigidbody2D>().velocity.x, 0f);
            player.GetComponent<Rigidbody2D>().AddForce(new Vector2(0f, 500.0f));
            //On joue l'animation de mort?
            
            //On d�truit l'ennemi apres l'animation de mort
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
            playerHealth.TakeDamage(1.0f);
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
