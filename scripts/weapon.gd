extends Node3D

var shot_counter: float = 0
var shot_timer_limit: float = 0.3
var shot_timer_reset: float = -0.5
var can_shotgun := false
@onready var raycast: RayCast3D = $"../../DistanceToGround"
@onready var father := get_parent().get_parent() as Player

func _physics_process(delta: float) -> void:
	handle_shoot(delta)


func get_angle_force() -> float:
	var ret_float := global_rotation.distance_to(Vector3.DOWN)
	return ret_float


func handle_shoot(delta: float) -> void:
	if shot_counter < 0:
		shot_counter = min(shot_counter + delta * 2, 0)
	if Input.is_action_pressed("ui_shot"):
		shot_counter = min(shot_counter + delta, shot_timer_limit)
		can_shotgun = true
		
		if shot_counter == shot_timer_limit:
			continuous_shot(delta)
			can_shotgun = true
	if Input.is_action_just_released("ui_shot"):
		if shot_counter == shot_timer_limit:
			shot_counter = 0
			$WaterSpray.emitting = false
		else:
			if can_shotgun:
				can_shotgun = false
				$WaterSpray.emitting = false
				shotgun(delta)
				shot_counter = shot_timer_reset


func continuous_shot(delta) -> void:
	$WaterSpray.randomness = 0
	$WaterSpray.explosiveness = 0
	$WaterSpray.emitting = true
	
	
	var direction: Vector3 = global_transform.basis.z
	direction *= 1.5
	if not father.is_on_floor():
		if raycast.is_colliding():
			var distance := raycast.get_collision_point().distance_to(father.position)
			var y_mult := remap(distance, 3, 1, 0.1, 1)
			
			direction.y *= y_mult
		else:
			direction.y *= 0.05
	else:
		direction.y = 0
	
	father.velocity += direction


func shotgun(delta) -> void:
	$ShotgunSpray.restart()
	
	var direction: Vector3 = global_transform.basis.z
	direction *= 20
	if not father.is_on_floor():
		if raycast.is_colliding():
			var distance := raycast.get_collision_point().distance_to(father.position)
			var y_mult := remap(distance, 3, 0, 0.1, 0.7)
			
			direction.y *= y_mult
		else:
			direction.y *= 0.05
	else:
		direction.y = 0
	
	father.velocity += direction
	
