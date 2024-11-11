@tool
extends EditorScript

var lightmap_texel_size = 0.4

func _run():
	for node in get_all_children(get_scene()):
		if node is MeshInstance3D:
			# Don't operate on instanced subscene children, as changes are lost
			# when reloading the scene.
			# See the "Instancing scenes" section below for a description of `owner`.
			var is_instanced_subscene_child = node != get_scene() and node.owner != get_scene()
			if not is_instanced_subscene_child:
				node.mesh = node.mesh.duplicate()
				process_uv2(node)

# This function is recursive: it calls itself to get lower levels of child nodes as needed.
# `children_acc` is the accumulator parameter that allows this function to work.
# It should be left to its default value when you call this function directly.
func get_all_children(in_node, children_acc = []):
	children_acc.push_back(in_node)
	for child in in_node.get_children():
		children_acc = get_all_children(child, children_acc)

	return children_acc
	

func process_uv2(node: MeshInstance3D):
	var old_mesh = node.mesh as Mesh
	node.mesh = ArrayMesh.new()
	for surface_id in range(old_mesh.get_surface_count()):
		node.mesh.add_surface_from_arrays(
			Mesh.PRIMITIVE_TRIANGLES, 
			old_mesh.surface_get_arrays(surface_id)
		)
		var old_mat = old_mesh.surface_get_material(surface_id)
		node.mesh.surface_set_material(surface_id, old_mat)

	node.mesh.lightmap_unwrap(node.global_transform, lightmap_texel_size)
	node.use_in_baked_light = true
	node.mesh.set_blend_shape_mode(Mesh.BLEND_SHAPE_MODE_RELATIVE)
