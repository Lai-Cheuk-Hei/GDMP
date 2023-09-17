using Godot;
using Godot.Collections;
using System;

namespace GDMP
{
	[Tool]
	public partial class SourceGenerator : EditorScript
	{
		public override void _Run()
		{
            GodotObject control = new GodotObject();
			var str = GenerateClassSource("MediaPipeFaceDetector");
			GD.Print(str);
		}

		private string GenerateClassSource(string className)
		{
			if (!ClassDB.ClassExists(className))
				throw new Exception();
			Array<Dictionary> signals = ClassDB.ClassGetSignalList(className, true);
			Array<Dictionary> methods = ClassDB.ClassGetMethodList(className, true);
			string source =
$@"using Godot;
using System;

public partial class {className} : RefCounted
{{
	public GodotObject Instance {{get; private set;}}

    public {className}()
    {{
        Instance = (GodotObject)ClassDB.Instantiate(""{className}"");
    }}

	public {className}(GodotObject instance)
	{{
		Instance = instance;
	}}
";
			foreach (Dictionary method in methods)
			{
                Dictionary method_return = (Dictionary)method["return"];
				string return_type = GetType(method_return);
                string return_class = return_type;
                if ((int)method_return["type"] == (int)Variant.Type.Object)
                    return_class = (string)method_return["class_name"];
				string method_name = (string)method["name"];
				Array<Dictionary> method_args = (Array<Dictionary>)method["args"];
				string args = "";
                string call_args = "";
				foreach (Dictionary arg in method_args)
				{
                    if (args.Length != 0)
                        args += ", ";
                    string argName = (string)arg["name"];
                    string type = GetType(arg);
					args += $@"{type} {argName}";
                    call_args += $@", {argName}";
				}
                string method_call = $@"Instance?.Call(""{method_name}""{call_args})";
                method_call = $@"({return_type}){method_call}";
                if ((int)method_return["type"] == (int)Variant.Type.Object)
                    method_call = $@"new {return_class}({method_call})";
                string return_string = return_type == "void" ? "" : "return ";
				source += $@"
	public {return_class} {method_name.ToPascalCase()}({args})
	{{
        {return_string}{method_call};
    }}
";
			}
			source += $@"
}}
";
			return source;
		}

		private string GetType(Dictionary d)
		{
            int type = (int)d["type"];
            string className = (string)d["class_name"];
			switch (type)
			{
				case (int)Variant.Type.Nil:
					if (className != "")
						return className;
					break;
				case (int)Variant.Type.Bool:
					return "bool";
                case (int)Variant.Type.Int:
                    return "int";
                case (int)Variant.Type.Rect2:
                    return "Rect2";
				case (int)Variant.Type.Object:
					return "GodotObject";
			}
			return "void";
		}
	}
}
