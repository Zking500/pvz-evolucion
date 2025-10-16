using UnityEngine;

public class PlayerMoved : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float jumpForce = 8f;
    public float gravity = -20f;
    
    [Header("Shooting")]
    public GameObject spherePrefab;
    public Transform shootPoint;
    public float shootForce = 20f;
    public float shootCooldown = 0.3f;
    
    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;
    private float lastShootTime;
    private Camera playerCamera;
    
    void Start()
    {
        // Setup CharacterController
        controller = GetComponent<CharacterController>();
        if (controller == null)
        {
            controller = gameObject.AddComponent<CharacterController>();
        }
        
        // Configuración corregida para evitar estar bajo tierra
        controller.center = new Vector3(0, 1, 0);
        controller.height = 2f;
        controller.radius = 0.5f;
        controller.skinWidth = 0.08f; // Prevenir atravesar el suelo
        controller.minMoveDistance = 0.001f;
        
        // Asegurar que el jugador esté a la altura correcta
        if (controller.isGrounded)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y + 0.1f, transform.position.z);
        }
        
        // Setup Camera
        playerCamera = GetComponentInChildren<Camera>();
        if (playerCamera == null)
        {
            GameObject cameraObj = new GameObject("PlayerCamera");
            cameraObj.transform.SetParent(transform);
            cameraObj.transform.localPosition = new Vector3(0, 1.5f, 0);
            playerCamera = cameraObj.AddComponent<Camera>();
        }
        
        // Setup Shoot Point
        if (shootPoint == null)
        {
            GameObject shootObj = new GameObject("ShootPoint");
            shootObj.transform.SetParent(playerCamera.transform);
            shootObj.transform.localPosition = new Vector3(0, 0, 0.5f);
            shootPoint = shootObj.transform;
        }
    }
    
    void Update()
    {
        HandleMovement();
        HandleShooting();
        FixUndergroundPosition(); // Corregir posición si está bajo tierra
    }
    
    void HandleMovement()
    {
        // Ground check
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        
        // Movimiento WASD
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        
        Vector3 move = transform.right * horizontal + transform.forward * vertical;
        controller.Move(move * moveSpeed * Time.deltaTime);
        
        // Salto
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
        }
        
        // Aplicar gravedad
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
        
        // Rotación con ratón
        float mouseX = Input.GetAxis("Mouse X");
        transform.Rotate(Vector3.up * mouseX * 2f);
    }
    
    void HandleShooting()
    {
        if (Input.GetMouseButtonDown(0) && Time.time >= lastShootTime + shootCooldown)
        {
            Shoot();
            lastShootTime = Time.time;
        }
    }
    
    void Shoot()
    {
        if (spherePrefab != null && shootPoint != null)
        {
            // Crear la esfera
            GameObject sphere = Instantiate(spherePrefab, shootPoint.position, shootPoint.rotation);

            // Asegurar que tenga Rigidbody
            Rigidbody rb = sphere.GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = sphere.AddComponent<Rigidbody>();
            }

            // Asegurar que tenga SphereProjectile
            SphereProjectile projectile = sphere.GetComponent<SphereProjectile>();
            if (projectile == null)
            {
                projectile = sphere.AddComponent<SphereProjectile>();
            }

            // Configurar el proyectil
            if (projectile != null)
            {
                projectile.SetDamage(25f); // Daño base
                projectile.SetLifeTime(3f); // Tiempo de vida
            }

            // Aplicar velocidad
            rb.linearVelocity = shootPoint.forward * shootForce;

            // Asegurar destrucción después del tiempo de vida
            Destroy(sphere, 3f);
        }
    }
    
    void FixUndergroundPosition()
    {
        // Verificar si el jugador está bajo tierra
        if (controller != null && controller.enabled)
        {
            RaycastHit hit;
            Vector3 rayStart = transform.position + Vector3.up * 0.1f;
            Vector3 rayDirection = Vector3.down;
            
            // Lanzar rayo hacia abajo para detectar el suelo
            if (Physics.Raycast(rayStart, rayDirection, out hit, 10f))
            {
                float groundY = hit.point.y;
                float playerBottomY = transform.position.y - controller.height / 2 + controller.center.y - controller.radius;
                
                // Si el jugador está bajo el suelo, corregir posición
                if (playerBottomY < groundY)
                {
                    float correction = groundY - playerBottomY + 0.1f;
                    transform.position = new Vector3(transform.position.x, transform.position.y + correction, transform.position.z);
                }
            }
        }
    }
}