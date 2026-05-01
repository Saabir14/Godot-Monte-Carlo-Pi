using System;
using System.Diagnostics;
using Godot;

[GlobalClass]
public partial class MonteCarloGPUCompute : MonteCarlo
{
    public override int shots(int interval)
    {
        progress = -1;

        // Create rendering device
        var rd = RenderingServer.CreateLocalRenderingDevice();
        if (rd is null)
        {
            Debug.Print(
                "Couldn't create RenderingDevice on GPU: " + RenderingServer.GetVideoAdapterName(),
                "\nNote: RenderingDevice is only available in the Forward+ and Mobile rendering methods, not Compatibility."
            );
            return interval;
        }
        Debug.Print("Creating RenderingDevice on GPU: " + RenderingServer.GetVideoAdapterName());

        // Load GLSL shader
        var shaderFile = GD.Load<RDShaderFile>("res://shaders/shots_compute.glsl");
        Rid shader = rd.ShaderCreateFromSpirV(shaderFile.GetSpirV());

        // Prepare our data
        int[] input = {0};
        var inputBytes = new byte[sizeof(int)];
        Buffer.BlockCopy(input, 0, inputBytes, 0, inputBytes.Length);
        
        // Create storage buffer
        Rid buffer = rd.StorageBufferCreate((uint) inputBytes.Length, inputBytes);

        // Create uniform
        var uniform = new RDUniform
        {
            UniformType = RenderingDevice.UniformType.StorageBuffer,
            Binding = 0
        };
        uniform.AddId(buffer);
        Rid uniformSet = rd.UniformSetCreate([uniform], shader, 0);

        // Create shader pipeline
        Rid pipeline = rd.ComputePipelineCreate(shader);
        var computeList = rd.ComputeListBegin();
        rd.ComputeListBindComputePipeline(computeList, pipeline);
        rd.ComputeListBindUniformSet(computeList, uniformSet, 0);
        rd.ComputeListDispatch(computeList, xGroups: (uint) interval, yGroups: 1, zGroups: 1);
        rd.ComputeListEnd();

        // Submit to GPU
        rd.Submit();

        // Wait for Sync
        rd.Sync();

        var outputBytes = rd.BufferGetData(buffer);
        int[] output = new int[input.Length];
        Buffer.BlockCopy(outputBytes, 0, output, 0, outputBytes.Length);

        rd.FreeRid(buffer);
        rd.FreeRid(shader);
        rd.Free();

        progress = interval;
        return output[0];
    }
}
