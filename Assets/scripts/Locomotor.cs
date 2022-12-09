using UnityEngine;

[SelectionBase]
[RequireComponent(typeof(CharacterController))]
public class Locomotor : MonoBehaviour
{
    public float speed;
    public float stopDistance;
    public float smoothTime;
    
    [HideInInspector] public float targetDistance;

    public bool stopped;
    public bool shouldMove;
    
    public LayerMask whatIsTarget;
        
    private CharacterController locomotor;
    
    private Vector3 target = Vector3.zero;
    private Vector3 velocity = Vector3.zero;
    
    private Quaternion rotation = Quaternion.identity;

    private void Awake(){
        locomotor = GetComponent<CharacterController>();
    }

    private void Update(){
        // Create ray that points to where mouse is
        Ray mouse = Camera.main.ScreenPointToRay(Input.mousePosition);

        // Take target
        target = Physics.Raycast(mouse, out RaycastHit hit, 500, whatIsTarget) ? hit.point : Vector3.zero;

        // If target OK make car follow target
        if (target != Vector3.zero){
            float zTargetDistance = target.z - transform.position.z;
            float xTargetDistance = target.x - transform.position.x;

            float targetAngle = Mathf.Atan2(zTargetDistance, xTargetDistance);

            // If min distance isn't reached, we should interpolate towards our current rotation (car will remain in actual position)
            Vector3 newVel = Vector3.zero;
            Quaternion newRot = transform.localRotation;
            
            targetDistance = Vector3.Distance(transform.position, target);
            shouldMove = !stopped && (targetDistance >= stopDistance) && Application.isFocused;

            if (shouldMove){
                newVel = speed * Time.deltaTime * new Vector3(Mathf.Cos(targetAngle), 0,  Mathf.Sin(targetAngle));
                newRot = Quaternion.Euler(0, -(targetAngle * Mathf.Rad2Deg) + 90, 0);  // Rotate car, note we convert theta which is in rads to degs, negatiev it, and add 90 to make model rotation line up
            }
                            
            // Smooth out velocity change
            velocity = Vector3.Lerp(velocity, newVel, smoothTime * Time.deltaTime);
            rotation = Quaternion.Lerp(rotation, newRot, smoothTime * Time.deltaTime);

            // Apply movement and rotation
            locomotor.Move(velocity);
            transform.localRotation = rotation;
        }
    }
}
