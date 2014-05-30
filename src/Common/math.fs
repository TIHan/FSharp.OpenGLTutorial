﻿(*
Copyright (c) 2013-2014 William F. Smith

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.

Influenced by:
    The Open Toolkit library.
    OpenGL Mathematics (glm.g-truc.net)
*)

namespace FSharp.Game.Math

#nowarn "9"

open System
open System.Numerics
open System.Collections.Generic
open System.Runtime.InteropServices

/// Math Module
[<RequireQualifiedAccess>]
module Math =
    [<Literal>]
    let PI = 3.14159265358979323846f

    [<Literal>]
    let E = 2.7182818284590452354f

    [<Literal>]
    let ``PI / 360`` = 0.00872664625997165f
        
    let inline lerp x y t = x + (y - x) * t

[<AutoOpen>]
module Operators =
    let inline clamp x min max =
        match x with
        | _ when x < min -> min
        | _ when x > max -> max
        | _ -> x

/// Single with Units of Measure
type single<[<Measure>] 'Measure> = float32<'Measure>

/// Degrees
[<Measure>] type deg

/// Radians
[<Measure>] type rad

[<RequireQualifiedAccess>]
module Deg =
    [<Literal>]
    let ``PI / 180`` = 0.0174532925199433f<rad/deg>

    let inline toRad x : single<rad> = x * ``PI / 180``

[<RequireQualifiedAccess>]
module Rad =
    [<Literal>]
    let ``180 / PI`` = 57.2957795130823f<deg/rad>    

    let inline toDeg x : single<deg> = x * ``180 / PI``

[<AutoOpen>]
module Vector2f =
    type Vector2f with
        member inline this.Item
            with get (i) =
                match i with
                | 0 -> this.X | 1 -> this.Y
                | _ -> raise <| IndexOutOfRangeException ()
    and vec2 = Vector2f

[<AutoOpen>]
module Vector3f =
    type Vector3f with
        member inline this.Item
            with get (i) =
                match i with
                | 0 -> this.X | 1 -> this.Y | 2 -> this.Z
                | _ -> raise <| IndexOutOfRangeException ()

        member inline this.Set (?x: single, ?y: single, ?z: single) =
            vec3 (
                (match x with | Some x -> x | None -> this.X),
                (match y with | Some y -> y | None -> this.Y),
                (match z with | Some z -> z | None -> this.Z))

        member inline this.SquaredMagnitude = this.X * this.X + this.Y * this.Y + this.Z * this.Z
        member inline this.Magnitude = sqrt this.SquaredMagnitude

    and vec3 = Vector3f

[<AutoOpen>]
module Vector4f =
    type Vector4f with    
        member inline this.Item
            with get (i) =
                match i with
                | 0 -> this.X | 1 -> this.Y | 2 -> this.Z | 3 -> this.W
                | _ -> raise <| IndexOutOfRangeException ()
    and vec4 = Vector4f

open Vector2f
open Vector3f
open Vector4f

[<RequireQualifiedAccess>]
module Vec2 =    
    let zero =  vec2 (0.f)
    let one =   vec2 (1.f)
    let right = vec2 (1.f, 0.f)
    let up =    vec2 (0.f, 1.f)
    let left =  vec2 (-1.f, 0.f)
    let down =  vec2 (0.f, -1.f)

[<RequireQualifiedAccess>]
module Vec3 =
    let zero =      vec3 (0.f)
    let one =       vec3 (1.f)
    let right =     vec3 (1.f, 0.f, 0.f)
    let up =        vec3 (0.f, 1.f, 0.f)
    let forward =   vec3 (0.f, 0.f, 1.f)
    let left =      vec3 (-1.f, 0.f,  0.f)
    let down =      vec3 (0.f, -1.f,  0.f)
    let back =      vec3 (0.f,  0.f, -1.f)

    let inline abs (v: vec3) = vec3 (abs v.X, abs v.Y, abs v.Z)

    let inline truncate (v: vec3) = vec3 (truncate v.X, truncate v.Y, truncate v.Z)

    let inline floor (v: vec3) = vec3 (floor v.X, floor v.Y, floor v.Z)

    // FIXME: hacky
    let inline minDimension (v: vec3) =
        match v.X < v.Y with
        | true ->
            match v.X < v.Z with
            | true -> 0
            | _ -> 2
        | _ ->
            match v.Y < v.Z with
            | true -> 1
            | _ -> 2
        
    let inline multiplyAdd s (v1: vec3) (v2: vec3) =
        vec3 (s * v1.X + v2.X, s * v1.Y + v2.Y, s * v1.Z + v2.Z)

    let inline dot (v1: vec3) (v2: vec3) =
        v1.X * v2.X + v1.Y * v2.Y + v1.Z * v2.Z

    let inline cross (v1: vec3) (v2: vec3) =
        vec3 (
            v1.Y * v2.Z - v1.Z * v2.Y,
            v1.Z * v2.X - v1.X * v2.Z,
            v1.X * v2.Y - v1.Y * v2.X)

    let inline squaredMagnitude (v: vec3) = v.SquaredMagnitude

    let inline magnitude (v: vec3) = v.Magnitude

    let inline normalize v =
        let length = 1.f / magnitude v
        vec3 (v.X * length, v.Y * length, v.Z * length)

    // FIXME: This is kinda of all hacky to begin with.
    let inline perpendicular v =
        let uv =
            match abs v |> minDimension with
            | 0 -> right | 1 -> up | 2 -> forward
            | _ -> raise <| System.ArgumentOutOfRangeException ()

        let vn = normalize uv
        let result = cross v vn
        match v.Y < 0.f with
        | true -> -1.f * result
        | _ -> result

    let inline lerp (v1: vec3) (v2: vec3) (t: single) =
        vec3 (Math.lerp v1.X v2.X t, Math.lerp v1.Y v2.Y t, Math.lerp v1.Z v2.Z t)

[<RequireQualifiedAccess>]
module Vec4 =
    let zero =  vec4 (0.f)
    let one =   vec4 (1.f)

    let inline dot (v1: vec4) (v2:vec4) =
        v1.X * v2.X + v1.Y * v2.Y + v1.Z * v2.Z + v1.W * v2.W

[<Struct>]
type Matrix2 =
    val Row1 : vec2
    val Row2 : vec2

    new (m11, m12, m21, m22) = { 
        Row1 = vec2 (m11, m12)
        Row2 = vec2 (m21, m22) }

    new (row1, row2) = {
        Row1 = row1
        Row2 = row2 }

    new (value) = {
        Row1 = vec2 value
        Row2 = vec2 value }

    member inline this.M11 = this.Row1.X
    member inline this.M12 = this.Row1.Y
    member inline this.M21 = this.Row2.X
    member inline this.M22 = this.Row2.Y

    member inline this.Item
            with get (i, j) =
                match (i, j) with
                | (0, 0) -> this.M11 | (0, 1) -> this.M12
                | (1, 0) -> this.M21 | (1, 1) -> this.M22
                | _ -> raise <| IndexOutOfRangeException ()
and mat2 = Matrix2

[<Struct>]
type Matrix3 =
    val Row1 : vec3
    val Row2 : vec3
    val Row3 : vec3   

    new (m11, m12, m13, m21, m22, m23, m31, m32, m33) = {
        Row1 = vec3 (m11, m12, m13)
        Row2 = vec3 (m21, m22, m23)
        Row3 = vec3 (m31, m32, m33) }

    new (row1, row2, row3) = {
        Row1 = row1
        Row2 = row2
        Row3 = row3 }

    new (value) = {
        Row1 = vec3 value
        Row2 = vec3 value
        Row3 = vec3 value }

    member inline this.M11 = this.Row1.X
    member inline this.M12 = this.Row1.Y
    member inline this.M13 = this.Row1.Z
    member inline this.M21 = this.Row2.X
    member inline this.M22 = this.Row2.Y
    member inline this.M23 = this.Row2.Z
    member inline this.M31 = this.Row3.X
    member inline this.M32 = this.Row3.Y
    member inline this.M33 = this.Row3.Z
    
    member inline this.Item
            with get (i, j) =
                match (i, j) with
                | (0, 0) -> this.M11 | (0, 1) -> this.M12 | (0, 2) -> this.M13
                | (1, 0) -> this.M21 | (1, 1) -> this.M22 | (1, 2) -> this.M23
                | (2, 0) -> this.M31 | (2, 1) -> this.M32 | (2, 2) -> this.M33
                | _ -> raise <| IndexOutOfRangeException ()
and mat3 = Matrix3

type mat4 = Matrix4
and [<Struct>] Matrix4 =
    val Row1 : vec4
    val Row2 : vec4
    val Row3 : vec4  
    val Row4 : vec4

    member inline this.Column1 = vec4 (this.Row1.X, this.Row2.X, this.Row3.X, this.Row4.X)
    member inline this.Column2 = vec4 (this.Row1.Y, this.Row2.Y, this.Row3.Y, this.Row4.Y)
    member inline this.Column3 = vec4 (this.Row1.Z, this.Row2.Z, this.Row3.Z, this.Row4.Z)
    member inline this.Column4 = vec4 (this.Row1.W, this.Row2.W, this.Row3.W, this.Row4.W)       

    new (m11, m12, m13, m14, m21, m22, m23, m24, m31, m32, m33, m34, m41, m42, m43, m44) = {
        Row1 = vec4 (m11, m12, m13, m14)
        Row2 = vec4 (m21, m22, m23, m24)
        Row3 = vec4 (m31, m32, m33, m34)
        Row4 = vec4 (m41, m42, m43, m44) }

    new (value) = {
        Row1 = vec4 (value, 0.f, 0.f, 0.f)
        Row2 = vec4 (0.f, value, 0.f, 0.f)
        Row3 = vec4 (0.f, 0.f, value, 0.f)
        Row4 = vec4 (0.f, 0.f, 0.f, value) }

    member inline this.M11 = this.Row1.X
    member inline this.M12 = this.Row1.Y
    member inline this.M13 = this.Row1.Z
    member inline this.M14 = this.Row1.W
    member inline this.M21 = this.Row2.X
    member inline this.M22 = this.Row2.Y
    member inline this.M23 = this.Row2.Z
    member inline this.M24 = this.Row2.W
    member inline this.M31 = this.Row3.X
    member inline this.M32 = this.Row3.Y
    member inline this.M33 = this.Row3.Z
    member inline this.M34 = this.Row3.W
    member inline this.M41 = this.Row4.X
    member inline this.M42 = this.Row4.Y
    member inline this.M43 = this.Row4.Z
    member inline this.M44 = this.Row4.W
    
    member inline this.Item
            with get (i, j) =
                match (i, j) with
                | (0, 0) -> this.M11 | (0, 1) -> this.M12 | (0, 2) -> this.M13 | (0, 3) -> this.M14
                | (1, 0) -> this.M21 | (1, 1) -> this.M22 | (1, 2) -> this.M23 | (1, 3) -> this.M24
                | (2, 0) -> this.M31 | (2, 1) -> this.M32 | (2, 2) -> this.M33 | (2, 3) -> this.M34
                | (3, 0) -> this.M41 | (3, 1) -> this.M42 | (3, 2) -> this.M43 | (3, 3) -> this.M44
                | _ -> raise <| IndexOutOfRangeException ()    

    static member (*) (m1: mat4, m2: mat4) =
        let inline f i j = m1.[i, 0] * m2.[0, j] + m1.[i, 1] * m2.[1, j] + m1.[i, 2] * m2.[2, j] + m1.[i, 3] * m2.[3, j]
        mat4 (
            f 0 0, f 0 1, f 0 2, f 0 3,
            f 1 0, f 1 1, f 1 2, f 1 3,
            f 2 0, f 2 1, f 2 2, f 2 3,
            f 3 0, f 3 1, f 3 2, f 3 3)

    static member (*) (m: mat4, v: vec4) =
        let inline f i = m.[0, i] * v.X + m.[1, i] * v.Y + m.[2, i] * v.Z + m.[3, i] * v.W
        vec4 (f 0, f 1, f 2, f 3)

[<Struct>]
type Quaternion =
    val XYZ : vec3
    val W : single

    new (w, x, y, z) = { XYZ = vec3 (x, y, z); W = w }
    new (w, v: vec3) = { XYZ = v; W = w }

    member inline this.X = this.XYZ.X
    member inline this.Y = this.XYZ.Y
    member inline this.Z = this.XYZ.Z
        
    static member inline Dot (q1: quat, q2: quat) =
        q1.X * q2.X + q1.Y * q2.Y + q1.Z * q2.Z + q1.W * q2.W

    member inline this.Conjugate = quat (this.W, -this.X, -this.Y, -this.Z)

    member inline this.Magnitude = sqrt <| quat.Dot (this, this)

    static member (*) (q1: quat, q2: quat) =
        quat (
            (q1.W * q2.W - q1.X * q2.X - q1.Y * q2.Y - q1.Z * q2.Z),
            (q1.W * q2.X + q1.X * q2.W + q1.Y * q2.Z - q1.Z * q2.Y),
            (q1.W * q2.Y + q1.Y * q2.W + q1.Z * q2.X - q1.X * q2.Z),
            (q1.W * q2.Z + q1.Z * q2.W + q1.X * q2.Y - q1.Y * q2.X))

    /// Steps:
    /// normalize vector
    /// create quat based on normalized vector's x,y,z
    /// then multiply by the passed quat's conjugate (inverse)
    /// then multiply by the passed quat
    /// create vector based on result quat's x,y,z
    static member inline (*) (q: quat, v) =
        let q' = q * (quat (0.f, Vec3.normalize v) * q.Conjugate)
        vec3 (q'.X, q'.Y, q'.Z)
and quat = Quaternion

[<RequireQualifiedAccess>]
module Mat4 =
    let identity = mat4 (1.f)

    let transpose (m: mat4) =
        mat4 (
            m.M11, m.M21, m.M31, m.M41,
            m.M12, m.M22, m.M32, m.M42,
            m.M13, m.M23, m.M33, m.M43,
            m.M14, m.M24, m.M34, m.M44)

    let inline createTranslation x y z =
        mat4 (
            1.f, 0.f, 0.f, 0.f,
            0.f, 1.f, 0.f, 0.f,
            0.f, 0.f, 1.f, 0.f,
            x, y, z, 1.f)

    let inline createScaling x y z =
        mat4 (
            x,   0.f, 0.f, 0.f,
            0.f, y,   0.f, 0.f,
            0.f, 0.f, z,   0.f,
            0.f, 0.f, 0.f, 1.f)

    let inline createRotation axis angle =
        let angle = Deg.toRad angle * 1.f</rad>
        let axis = Vec3.normalize axis

        let c = cos -angle
        let s = sin -angle
        let t = 1.f - c

        mat4 (
            t * axis.X * axis.X + c, t * axis.X * axis.Y - s * axis.Z, t * axis.X * axis.Z + s * axis.Y, 0.f,
            t * axis.X * axis.Y + s * axis.Z, t * axis.Y * axis.Y + c, t * axis.Y * axis.Z - s * axis.X, 0.f,
            t * axis.X * axis.Z - s * axis.Y, t * axis.Y * axis.Z + s * axis.X, t * axis.Z * axis.Z + c, 0.f,
            0.f, 0.f, 0.f, 1.f)

    let inline createPerspective fovY aspect zNear zFar =
        let fovY = Deg.toRad fovY
        let tanHalfFovY = tan <| fovY / 2.f<rad>

        mat4 (
            1.f / (aspect * tanHalfFovY), 0.f, 0.f, 0.f,
            0.f, 1.f / tanHalfFovY, 0.f, 0.f,
            0.f, 0.f, - (zFar + zNear) / (zFar - zNear), -1.f,
            0.f, 0.f, - (2.f * zFar * zNear) / (zFar - zNear), 0.f)

    let inline lookAt eye center up =
        let f = Vec3.normalize (center - eye)
        let s = Vec3.cross f up |> Vec3.normalize
        let u = Vec3.cross s f

        mat4 (
            s.X, u.X, -f.X, 0.f,
            s.Y, u.Y, -f.Y, 0.f,
            s.Z, u.Z, -f.Z, 0.f,
            -Vec3.dot s eye, -Vec3.dot u eye, Vec3.dot f eye, 1.f)

[<RequireQualifiedAccess>]
module Quat =
    let inline dot q1 q2 = quat.Dot (q1, q2)

    let inline conjugate (q: quat) = q.Conjugate

    let inline magnitude (q: quat) = q.Magnitude

    let inline normalize (q: quat) =
        let ``1 / length`` = 1.f / magnitude q
        quat (
            (q.W * ``1 / length``),
            (q.X * ``1 / length``),
            (q.Y * ``1 / length``),
            (q.Z * ``1 / length``))

    let inline ofEulerDegrees (v: vec3) =
        let pitch = Math.``PI / 360`` * v.[0]
        let yaw =   Math.``PI / 360`` * v.[1]
        let roll =  Math.``PI / 360`` * v.[2]

        let sinRoll =   sin roll
        let sinPitch =  sin pitch
        let sinYaw =    sin yaw

        let cosRoll =   cos roll
        let cosPitch =  cos pitch
        let cosYaw =    cos yaw

        let cosPitchYaw = cosPitch * cosYaw
        let sinPitchYaw = sinPitch * sinYaw

        quat (
            (cosRoll * cosPitchYaw + sinRoll * sinPitchYaw),
            (sinRoll * cosPitchYaw - cosRoll * sinPitchYaw),
            (cosRoll * sinPitch * cosYaw + sinRoll * cosPitch * sinYaw),
            (cosRoll * cosPitch * sinYaw - sinRoll * sinPitch * cosYaw))

    let inline ofAxisAngle (axis: vec3) (angle: single<rad>) =
        let angle' = angle * 0.5f</rad>

        quat (
            (cos angle'),
            (sin angle' * Vec3.normalize axis))
        |> normalize

[<RequireQualifiedAccess>]
module Transform =
    let inline rotateAroundPoint point axis angle : vec3 =
        let q = Quat.ofAxisAngle axis <| Deg.toRad angle
        q * point
