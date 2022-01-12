namespace Synthesizer

open System
open System.IO

module API =
    
    let getNoteFreq octav note =
        CalcNoteFreq(octav, note).Output

    let getNoteFreqOffset octav note aFourFreq =
        CalcNoteFreq(octav, note, aFourFreq).Output


    let createSound freq duration waveType =
        createSoundData(frequency0 = freq, duration0 = duration).create(waveType)

    let writeToWav path data =
        writeWav().Write (File.Create(path)) (data)

    let note duration note octave =
        let freq = getNoteFreq note octave
        let soundData = createSound freq duration Sin
        writeToWav "wave.wav" soundData

    