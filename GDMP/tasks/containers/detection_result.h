#ifndef GDMP_TASK_DETECTION
#define GDMP_TASK_DETECTION

#include "Array.hpp"
#include "Godot.hpp"
#include "Rect2.hpp"
#include "Reference.hpp"

#include "mediapipe/tasks/cc/components/containers/detection_result.h"

#include "GDMP/tasks/containers/category.h"
#include "GDMP/tasks/containers/keypoint.h"

using namespace godot;
using namespace mediapipe::tasks::components::containers;

class MediaPipeDetection : public Reference {
		GODOT_CLASS(MediaPipeDetection, Reference)

	private:
		Detection detection;

	public:
		static void _register_methods();
		static MediaPipeDetection *_new(const Detection &detection);

		void _init();

		Array get_categories();
		Rect2 get_bounding_box();
		Array get_keypoints();

		bool has_keypoints();
};

class MediaPipeDetectionResult : public Reference {
		GODOT_CLASS(MediaPipeDetectionResult, Reference)

	private:
		DetectionResult result;

	public:
		static void _register_methods();
		static MediaPipeDetectionResult *_new(const DetectionResult &result);

		void _init();

		Array get_detections();
};

#endif
