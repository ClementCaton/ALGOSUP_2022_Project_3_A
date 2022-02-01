namespace Synthesizer

open System
open System.IO

module Synth =
    
    let GetNoteFreq octav note =
        CalcNoteFreq(octav, note).Output

    let GetNoteFreqOffset octav note aFourFreq =
        CalcNoteFreq(octav, note, aFourFreq).Output

    let Sound freq duration waveType =
        let data = SoundData(frequency0 = freq, duration0 = duration, bpm0 = 114) // TEMP: Remove bpm
        //! The "1." was supposed to be "(data.overDrive)"
        Utility.Overdrive 1. (data.Create(waveType))

    let SoundWithEnveloppe freq duration waveType sustain attack hold decay release = // time, time, time, amp, time
        let data = SoundData(frequency0 = freq, duration0 = duration, bpm0 = 114) // TEMP: Remove bpm
        //! The "1." was supposed to be "(data.overDrive)"
        Utility.Overdrive 1. (data.CreateWithEnvelope waveType sustain attack hold decay release)

    let WriteToWav name music =
        Directory.CreateDirectory("./Output/") |> ignore
        use Stream = File.Create("./Output/" + name)
        WriteWav().Write (Stream) (music)

    let WriteToWavWithPath path fileName music =
        Directory.CreateDirectory(path) |> ignore
        use Stream = File.Create(path + fileName)
        WriteWav().Write (Stream) (music)

    let ReadFromWav name =
        ReadWav().Read (File.Open("./Output/"+name, FileMode.Open))

    let ReadFromWavWithPath path =
        ReadWav().Read (File.Open(path, FileMode.Open))

    let Note duration mNote octave =
        let freq = GetNoteFreq mNote octave
        Sound freq duration Sin

    
    let Silence duration =
        Sound 0 duration Silence
        
    let ComposeCutCorner (corner:int) sounds =
        sounds |> List.map(fun x -> Utility.CutCorners corner x) |> List.concat
            
    let Compose = ComposeCutCorner 100
    
    let ComposeNoCutCorner sounds = List.concat
    
    let Add sounds = Utility.Add sounds

    let Preview title sound =
        Previewarr.chart title sound
        sound

    let PreviewMap title map =
        map
        |> Map.toList
        |> List.unzip
        ||> Previewarr.ChartXY title
        map

    let ForAllChannels func channels =
        channels |> List.map func

    let Fourier wave =
        FrequencyAnalysis.Fourier(wave)

    let Cutstart (sampleRate:float) time (data:List<float>) =
        Utility.CutStart sampleRate time data

    let CutEnd (sampleRate:float) time (data:List<float>) =
        Utility.CutEnd sampleRate time data

    let CutMiddle (sampleRate:float) timeStart timeEnd (data:List<float>) =
        Utility.CutStart sampleRate timeStart data
        |> List.append (Utility.CutEnd sampleRate timeEnd data)

    let CutEdge (sampleRate:float) timeStart timeEnd (data:List<float>) =
        Utility.CutEnd sampleRate timeStart data
        |> List.append (Utility.CutStart sampleRate timeEnd data)
        
    let CutCorners limit (data:List<float>) =
        Utility.CutCorners limit data

    
    