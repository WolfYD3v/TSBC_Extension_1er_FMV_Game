shader_type canvas_item;

uniform float strength: hint_range(0., 1.) = 0.5;
uniform float speed: hint_range(0., 10.) = 0.5;
uniform vec4 color1: source_color;
uniform vec4 color2: source_color;
uniform float x_target: hint_range(0., 1.) = 0.75;
uniform float y_target: hint_range(0.0, 1.) = 0.25;

void fragment() {
	vec2 center = vec2(x_target, y_target);
    float distance = length(UV - center);
    float hue = abs(fract(distance + fract(TIME * speed) - 0.5) * 2.0 - 1.0);
    hue = smoothstep(0.3, 0.7, hue); // Apply smoothstep to hue
    vec4 gradient_color = mix(color1, color2, hue);
    vec4 color = texture(TEXTURE, UV);
    if (color.a == 0.0) {
        COLOR = color;
    } else {
        COLOR = mix(color, gradient_color, strength);
    }
}