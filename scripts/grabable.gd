class_name Grabable
extends RigidBody3D

@export var explode_on_collision: bool = false

var has_been_thrown: bool = false


func approach_position(pos: Vector3) -> void:
	var dir := pos - global_position  
	var force = dir.length()
	apply_central_force(dir * pow(force, 2) * 10)

func throw(dir: Vector3) -> void:
	set_has_been_thrown(true)
	apply_central_force(dir * 750)

func grab() -> void:
	sleeping = true
	linear_velocity = Vector3.ZERO

func drop() -> void:
	sleeping = false

func set_has_been_thrown(val: bool) -> void:
	has_been_thrown = val


func _on_body_entered(body: Node) -> void:
	if not has_been_thrown and explode_on_collision:
		return
	else:
		VfxManager.spawn_lowres_explosion(position)
		queue_free()
