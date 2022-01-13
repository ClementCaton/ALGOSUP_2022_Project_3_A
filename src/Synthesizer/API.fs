namespace Synthesizer

open System
open System.IO

module API =
    
    let getNoteFreq octav note =
        CalcNoteFreq(octav, note).Output

    let getNoteFreqOffset octav note aFourFreq =
        CalcNoteFreq(octav, note, aFourFreq).Output


    let createSound freq duration waveType =
        createSoundData(frequency0 = freq, duration0 = duration, bpm0 = 114).create(waveType) // TEMP: Remove bpm

    let writeToWav path music =
        writeWav().Write (File.Create(path)) (music)

    let note duration note octave =
        let freq = getNoteFreq note octave
        createSound freq duration Sin
    
    let silence duration =
        createSound 0 duration Silence
    
    let compose =
        Array.concat

