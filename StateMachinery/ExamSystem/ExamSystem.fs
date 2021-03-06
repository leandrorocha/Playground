﻿namespace ExamSystem

module StateManager = 

    open System

    type Evaluation = { UserId : int; ChecklistId :int }

    type StateRequest = 
        | StudentExam of Evaluation list
        | FacultyExam of Evaluation list
        | Recording

    type State = 
        | DoorNotes of StateRequest list
        | Encounter of StateRequest list
        | PostEncounter of StateRequest list
        | Followup of StateRequest list
        | Empty

    type StateMessage = 
        | Forward
        | Reverse

    type RoomStates = { 
                        ConsumedStates: State list;  
                        PendingStates: State list;
                        CurrentState: State
                      }

    type RecorderStatus = 
        | NoStatus
        | Recording
        | Streaming
        | Preview

    type Room = { RoomId: int; 
                  States: RoomStates;
                  RecorderStatus: RecorderStatus}

    let roomString room = sprintf "%A" room

    let advance states =
        match states.PendingStates with 
            | h::t -> 
                { 
                    ConsumedStates = states.CurrentState::states.ConsumedStates; 
                    PendingStates = t;
                    CurrentState = h
                }       
            | [] -> { states with CurrentState = State.Empty }            

    let reverse states = 
        match states.ConsumedStates with 
           | h::t -> 
                { 
                    ConsumedStates = t; 
                    PendingStates = states.CurrentState::states.PendingStates;
                    CurrentState = h
                }        
            | [] -> states

    let findRoom roomId rooms = List.find (fun (r:Room) -> r.RoomId = roomId) rooms
    
    let findRoomAndApply step roomId rooms = 
        let room = findRoom roomId rooms 
        (room, step room.States)