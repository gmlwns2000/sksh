# Spiky Shader Language Pre-Processor (sksh)

Spiky Shader Language Pre-Processor (sksh)

Project Spiky (c) 2018

-- Spiky ShaderLang (.SKSH) is file format that is an extension of GLSL (OpenGL Shading language). --

# Helps

- `help`    | Show Help Messages
- `vout`    | Vertex Shader Output Name
- `fout`    | Fragment Shader Output Name
- `def`     | Defines Custom Keywords

# Usages

`$ sksh.exe main.sksh -vout mainv.sksh -fout mainf.sksh -def SOMENICE -def NEWDEFINES`

# Reserved Defines

- Vertex Shader   | `VERTEX`, `GEO`, `GEOMETRY`
- Fragment Shader | `PIXEL`, `FRAG`, `FRAGMENT`