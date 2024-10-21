extends Node

@onready var explosion_scene := preload("res://scenes/vfx/low_res_explosion.tscn")

func spawn_lowres_explosion(pos: Vector3) -> void:
	var new_node := explosion_scene.instantiate()
	add_child(new_node)
	new_node.position = pos
