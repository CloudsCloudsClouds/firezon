extends Node3D

var shot_counter: float = 0
var shot_timer_limit: float = 0.3
var shot_timer_reset: float = -0.5
var can_shotgun := false
var obj_grab: Grabable

@onready var raycast: RayCast3D = $"../../DistanceToGround"
@onready var grabcast: RayCast3D = $"../Camera3D/GrabHolder"
@onready var grabposi: Node3D = $"../Camera3D/GrabHolder/Position"
@onready var father := get_parent().get_parent() as Player

@export var weapon_rotation_ammount: float = 0.01

enum STATES {
	NOTHING,
	GRABING,
	WEAPON,
}
var state := STATES.WEAPON

func _ready() -> void:
	grabcast.add_exception(father)

func _physics_process(delta: float) -> void:
	match state:
		STATES.WEAPON:
			if Input.is_action_just_pressed("ui_grab"):
				if grabcast.is_colliding():
					var is_grabable := grabcast.get_collider()
					if is_grabable is Grabable:
						obj_grab = is_grabable
						obj_grab.grab()
						state = STATES.GRABING
			handle_shoot(delta)
		STATES.GRABING:
			if obj_grab == null:
				state = STATES.WEAPON
				return
			if Input.is_action_just_pressed("ui_grab"):
				obj_grab.drop()
				obj_grab = null
				state = STATES.WEAPON
			elif Input.is_action_just_pressed("ui_shot"):
				var throw_dir := grabcast.global_position.direction_to(grabposi.global_position)
				obj_grab.throw(throw_dir + father.velocity.normalized())
				obj_grab = null
				state = STATES.WEAPON
			handle_grab(delta)
		_:
			assert(false)

func handle_grab(delta: float) -> void:
	if obj_grab != null:
		obj_grab.set_position(grabposi.global_position)
		obj_grab.linear_velocity = Vector3.ZERO

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
			var y_mult := remap(distance, 3, 0.3, 0.1, 1)
			
			direction.y *= y_mult
		else:
			direction.y *= 0.05
	else:
		direction.y = 0
	
	var hor_vec := Vector2(direction.x, direction.z)
	hor_vec /= max(father.velocity.length() / 5, 1)
	direction.x = hor_vec.x
	direction.z = hor_vec.y
	
	
	father.velocity += direction


func shotgun(delta) -> void:
	$ShotgunSpray.restart()
	
	var direction: Vector3 = global_transform.basis.z
	direction *= 30
	if not father.is_on_floor():
		if raycast.is_colliding():
			var distance := raycast.get_collision_point().distance_to(father.position)
			var y_mult := remap(distance, 3, 0, 0.1, 0.7)
			
			direction.y *= y_mult
		else:
			direction.y *= 0.05
	else:
		direction.y = 0
	
	var hor_vec := Vector2(direction.x, direction.z)
	hor_vec /= max(father.velocity.length() / 5, 1)
	direction.x = hor_vec.x
	direction.z = hor_vec.y
	
	
	father.velocity += direction

func weapon_tilt(input_x, input_y, delta) -> void:
	var inpt = lerp(Vector2(input_x, input_y), Vector2.ZERO, 10 * delta)
	rotation.z = lerp(rotation.z, inpt.x * weapon_rotation_ammount, 10 * delta)
	rotation.x = lerp(rotation.x, inpt.y * weapon_rotation_ammount, 10 * delta)
