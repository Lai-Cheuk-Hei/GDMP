extends Node

enum {
	CAMERA_FACING_FRONT,
	CAMERA_FACING_BACK
}

# Landmark
const NormalizedLandmark : NativeScript = preload("NormalizedLandmark.gdns")
const NormalizedLandmarkList : NativeScript = preload("NormalizedLandmarkList.gdns")

var plugin : Object
var image : Image = Image.new()
var packet_data : Dictionary = {}

func _enter_tree():
	match OS.get_name():
		"Android":
			if Engine.has_singleton("GDMP"):
				plugin = Engine.get_singleton("GDMP")
				plugin.init(get_instance_id())
		_:
			plugin = Node.new()
			plugin.set_script(load("res://addons/GDMP/GDMP.gdns"))
			plugin.connect("new_frame", self, "_on_new_frame")
			add_child(plugin)
	plugin.connect("on_new_proto", self, "_on_new_packet")
	plugin.connect("on_new_proto_vector", self, "_on_new_packet")

func _on_new_packet(stream_name : String, data) -> void:
	if packet_data.has(stream_name):
		for obj in packet_data[stream_name]:
			if is_instance_valid(obj):
				obj.call(packet_data[stream_name][obj], data)

func _on_new_frame(data, width, height) -> void:
	var channel : int = int(data.size()/width/height)
	data.resize(width*height*channel)
	if channel == 4:
		image.create_from_data(width, height, false, Image.FORMAT_RGBA8, data)
	else:
		image.create_from_data(width, height, false, Image.FORMAT_RGB8, data)

func init_graph(graph_path : String, input_side_packets : Dictionary = {}) -> void:
	packet_data.clear()
	plugin.initGraph(graph_path, input_side_packets)

func add_proto_callback(stream_name : String, target : Object, method : String) -> void:
	if not packet_data.has(stream_name):
		plugin.addProtoCallback(stream_name)
		packet_data[stream_name] = {}
	packet_data[stream_name][target] = method

func add_proto_vector_callback(stream_name : String, target : Object, method : String) -> void:
	if not packet_data.has(stream_name):
		plugin.addProtoVectorCallback(stream_name)
		packet_data[stream_name] = {}
	packet_data[stream_name][target] = method

func start_camera(index : int = 0) -> void:
	plugin.startCamera(index)

func close_camera() -> void:
	plugin.closeCamera()

func load_video(path : String):
	plugin.loadVideo(path)
