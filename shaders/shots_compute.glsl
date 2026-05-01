#[compute]
#version 450

// Compute Shader can process data in three dimensions. 
// This is essentially an efficient design intended for 1D, 2D, and 3D data structures.
layout(local_size_x = 1, local_size_y = 1, local_size_z = 1) in;

// Connecting our data to the GPU.
// set = Data type
// binding = GPU binding point
// std430 = GPU memory structure standard.
// restrict buffer DataBuffer = Buffer with restricted access for optimization purposes.
layout(set = 0, binding = 0, std430) restrict buffer DataBuffer {
    int data;
}
data_buffer;


// Generate random number between 0 and 1
// Hash‑based RNG – 32‑bit integer input → 0‑1 float output
float random(uint seed) {
    // 32‑bit mix function (Xorshift / Murmur‑style)
    seed ^= seed >> 16u;
    seed *= 0x7feb352du;
    seed ^= seed >> 15u;
    seed *= 0x846ca68bu;
    seed ^= seed >> 16u;

    // Convert to float in [0,1)
    // 0x3f800000 is the bit pattern for 1.0f; we keep only the mantissa bits.
    uint mantissa = seed & 0x007FFFFFu;         // keep 23 mantissa bits
    uint floatBits = mantissa | 0x3F800000u;    // set exponent to 127 (value 1.0)
    return uintBitsToFloat(floatBits) - 1.0;    // subtract 1 -> range [0,1]
}
    

bool shoot() {
    uint id = gl_GlobalInvocationID.x;
    float rand_x = random(id);
    float rand_y = random(id + 1u);
    return rand_x * rand_x + rand_y * rand_y <= 1;
}

void main() {
    if (shoot()) atomicAdd(data_buffer.data, 1);
}