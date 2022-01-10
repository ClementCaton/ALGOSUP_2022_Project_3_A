open System.IO

//#r "nuget: XPlot.Plotly"
//open XPlot.Plotly

type soundMetaData = {
    RIFFSize:int;

    headerSize: int;
    pcmFormat: int16;
    mono: int16;
    sampleRate: int;
    byteRate: int;
    blockAlign: int16;
    bitsPerSample: int16;
}

let soundMetaData = {
    RIFFSize = 36;

    headerSize = 16;
    pcmFormat = 1s;
    mono = 1s;
    sampleRate = 10000;
    byteRate = 10000;
    blockAlign = 1s;
    bitsPerSample = 8s;
}

/// Write WAVE PCM soundfile (8KHz Mono 8-bit)
type createSound stream (metaData:soundMetaData) =
    let sample x = (x + 1.)/2. * 255. |> byte 
    let data:byte[] = Array.init 10000 (fun i -> sin (float i/float 4 ) |> sample)

    use writer = new BinaryWriter(stream)
    // RIFF
    writer.Write("RIFF"B)
    let size = metaData.RIFFSize + data.Length in writer.Write(size)
    writer.Write("WAVE"B)
    // fmt
    writer.Write("fmt "B)
    let headerSize = metaData.headerSize in writer.Write(headerSize)
    let pcmFormat = metaData.pcmFormat in writer.Write(pcmFormat)
    let mono = metaData.mono in writer.Write(mono)
    let sampleRate = metaData.sampleRate in writer.Write(sampleRate)
    let byteRate = metaData.byteRate in writer.Write(byteRate)
    let blockAlign = metaData.blockAlign in writer.Write(blockAlign)
    let bitsPerSample = metaData.bitsPerSample in writer.Write(bitsPerSample)
    // data
    writer.Write("data"B)
    writer.Write(data.Length)
    writer.Write(data)


let stream = File.Create(@"tone.wav")
createSound stream soundMetaData 
