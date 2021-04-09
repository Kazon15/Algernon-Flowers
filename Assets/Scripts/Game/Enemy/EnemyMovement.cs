using Game.Config;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.AI;
using Util.Character;

namespace Game.Enemy
{
    public sealed class EnemyMovement : AbstractMovement
    {
        [SerializeField]
        private NavMeshAgent agent;

        [SerializeField, Range(0f, 1000f), Tooltip("На какое расстояние должен отходить бот.")]
        private float distanceFromPlayer = 100f;

        [SerializeField, Range(0f, 1000f),
         Tooltip("На каком расстоянии должен быть игрок, что-бы бот начал отходить.")]
        private float distanceToPlayer = 300f;

        [SerializeField, MinMaxSlider(0f, 1000f),
         Tooltip("На каком расстоянии будет держаться бот от игрока.")]
        private Vector2 rangeToPlayer = new Vector2(400f, 600f);

        private Collider2D _colliderObject;

        private float _defaultStoppingDistance;

        private Player.Player _player;

        private void Start()
        {
            _colliderObject = GetComponent<Collider2D>();

            if(agent == null) agent = GetComponent<NavMeshAgent>();

            agent.updateUpAxis = false;
            agent.updateRotation = false;
            agent.stoppingDistance = rangeToPlayer.x;
            agent.speed = agent.acceleration = Speed * 10;
            _defaultStoppingDistance = agent.stoppingDistance;

            _player = GameConfig.Instance.Player;
        }

        private void LateUpdate()
        {
            if(!_player || _colliderObject.isTrigger)
            {
                agent.enabled = false;
                return;
            }

            agent.enabled = true;

            var range = Vector2.Distance(transform.position, _player.transform.position);

            if(range > rangeToPlayer.y) return;

            var position = _player.transform.position;

            Rotate(position);

            if(range > rangeToPlayer.x)
            {
                agent.stoppingDistance = _defaultStoppingDistance;
                agent.SetDestination(position);

                #if UNITY_EDITOR
                Debug.DrawLine(transform.position, position);
                #endif
            }

            if(range > distanceToPlayer) return;

            agent.stoppingDistance = 0f;

            var thisPosition = transform.position;

            var targetPosition = Vector2.MoveTowards(thisPosition,
                                                     (thisPosition - position) * distanceFromPlayer,
                                                     distanceFromPlayer);

            agent.SetDestination(targetPosition);

            #if UNITY_EDITOR
            Debug.DrawLine(thisPosition, targetPosition);
            #endif
        }
    }
}