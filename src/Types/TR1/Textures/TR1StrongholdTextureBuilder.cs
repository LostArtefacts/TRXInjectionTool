using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.Textures;

public class TR1StrongholdTextureBuilder : TextureBuilder
{
    public override List<InjectionData> Build()
    {
        TR1Level stronghold = _control1.Read($"Resources/{TR1LevelNames.STRONGHOLD}");
        InjectionData data = InjectionData.Create(TRGameVersion.TR1, InjectionType.TextureFix, "stronghold_textures");
        CreateDefaultTests(data, TR1LevelNames.STRONGHOLD);

        data.RoomEdits.AddRange(CreateFillers(stronghold));
        data.RoomEdits.AddRange(CreateRefacings());
        data.RoomEdits.AddRange(CreateRotations());
        data.RoomEdits.AddRange(CreateShifts(stronghold));

        FixRoom13(stronghold, data);
        FixHubRoom(stronghold, data);
        FixPassport(stronghold, data);

        return new() { data };
    }

    private static List<TRRoomTextureCreate> CreateFillers(TR1Level stronghold)
    {
        return new()
        {
            new()
            {
                RoomIndex = 63,
                FaceType = TRMeshFaceType.TexturedTriangle,
                SourceRoom = 63,
                SourceIndex = 18,
                Vertices = new()
                {
                    stronghold.Rooms[63].Mesh.Rectangles[51].Vertices[3],
                    stronghold.Rooms[63].Mesh.Rectangles[54].Vertices[2],
                    stronghold.Rooms[63].Mesh.Rectangles[54].Vertices[1],
                }
            }
        };
    }

    private static List<TRRoomTextureReface> CreateRefacings()
    {
        return new()
        {
            new()
            {
                RoomIndex = 7,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceFaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 7,
                SourceIndex = 31,
                TargetIndex = 25,
            },
            new()
            {
                RoomIndex = 75,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceFaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 7,
                SourceIndex = 31,
                TargetIndex = 25,
            },
            new()
            {
                RoomIndex = 2,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceFaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 2,
                SourceIndex = 304,
                TargetIndex = 267,
            },
            new()
            {
                RoomIndex = 6,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceFaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 6,
                SourceIndex = 425,
                TargetIndex = 462,
            }
        };
    }

    private static List<TRRoomTextureRotate> CreateRotations()
    {
        return new()
        {
            Rotate(6, TRMeshFaceType.TexturedQuad, 462, 3),
            Rotate(27, TRMeshFaceType.TexturedTriangle, 3, 1),
        };
    }

    private static List<TRRoomTextureMove> CreateShifts(TR1Level stronghold)
    {
        return new()
        {
            new()
            {
                RoomIndex = 19,
                FaceType = TRMeshFaceType.TexturedQuad,
                TargetIndex = 11,
                VertexRemap = new()
                {
                    new()
                    {
                        NewVertexIndex = stronghold.Rooms[19].Mesh.Rectangles[9].Vertices[0],
                    },
                    new()
                    {
                        Index = 3,
                        NewVertexIndex = stronghold.Rooms[19].Mesh.Rectangles[9].Vertices[1],
                    },
                }
            },
        };
    }

    private static void FixRoom13(TR1Level stronghold, InjectionData data)
    {
        TR1RoomVertex copy1 = stronghold.Rooms[13].Mesh.Vertices[stronghold.Rooms[13].Mesh.Rectangles[15].Vertices[0]];
        TR1RoomVertex copy2 = stronghold.Rooms[13].Mesh.Vertices[stronghold.Rooms[13].Mesh.Rectangles[18].Vertices[0]];
        data.RoomEdits.Add(new TRRoomVertexCreate
        {
            RoomIndex = 13,
            Vertex = new()
            {
                Lighting = copy1.Lighting,
                Vertex = new()
                {
                    X = copy1.Vertex.X,
                    Y = (short)(copy1.Vertex.Y + 1024),
                    Z = copy1.Vertex.Z
                }
            }
        });
        data.RoomEdits.Add(new TRRoomVertexCreate
        {
            RoomIndex = 13,
            Vertex = new()
            {
                Lighting = copy2.Lighting,
                Vertex = new()
                {
                    X = copy2.Vertex.X,
                    Y = (short)(copy2.Vertex.Y + 1024),
                    Z = copy2.Vertex.Z
                }
            }
        });

        data.RoomEdits.Add(new TRRoomTextureCreate
        {
            RoomIndex = 13,
            FaceType = TRMeshFaceType.TexturedQuad,
            SourceRoom = 13,
            SourceIndex = 15,
            Vertices = new()
            {
                stronghold.Rooms[13].Mesh.Rectangles[15].Vertices[0],
                stronghold.Rooms[13].Mesh.Rectangles[18].Vertices[0],
                (ushort)(stronghold.Rooms[13].Mesh.Vertices.Count + 1),
                (ushort)stronghold.Rooms[13].Mesh.Vertices.Count,
            }
        });

        TR1RoomVertex copy3 = stronghold.Rooms[13].Mesh.Vertices[stronghold.Rooms[13].Mesh.Rectangles[246].Vertices[3]];
        data.RoomEdits.Add(new TRRoomVertexCreate
        {
            RoomIndex = 13,
            Vertex = new()
            {
                Lighting = copy3.Lighting,
                Vertex = new TRVertex
                {
                    X = copy3.Vertex.X,
                    Y = (short)(copy3.Vertex.Y + 256),
                    Z = copy3.Vertex.Z
                }
            }
        });

        data.RoomEdits.Add(new TRRoomTextureCreate
        {
            RoomIndex = 13,
            FaceType = TRMeshFaceType.TexturedQuad,
            SourceRoom = 13,
            SourceIndex = 161,
            Vertices = new()
            {
                stronghold.Rooms[13].Mesh.Rectangles[216].Vertices[2],
                (ushort)(stronghold.Rooms[13].Mesh.Vertices.Count + 2),
                stronghold.Rooms[13].Mesh.Rectangles[246].Vertices[3],
                stronghold.Rooms[13].Mesh.Rectangles[246].Vertices[2],
            }
        });

        data.RoomEdits.Add(new TRRoomTextureCreate
        {
            RoomIndex = 13,
            FaceType = TRMeshFaceType.TexturedTriangle,
            SourceRoom = 13,
            SourceIndex = 59,
            Vertices = new()
            {
                (ushort)(stronghold.Rooms[13].Mesh.Vertices.Count + 2),
                stronghold.Rooms[13].Mesh.Rectangles[289].Vertices[2],
                stronghold.Rooms[13].Mesh.Rectangles[289].Vertices[1],
            }
        });
    }

    private static void FixHubRoom(TR1Level stronghold, InjectionData data)
    {
        // This fills the one-click gaps around the perimiter of the base of room 5 and surroundings, plus flipmap.

        List<TRFace> rects = stronghold.Rooms[5].Mesh.Rectangles;
        ushort nextVert = (ushort)stronghold.Rooms[5].Mesh.Vertices.Count;

        data.RoomEdits.Add(MakeHubVertex(stronghold, 5, 708, 3));
        data.RoomEdits.Add(MakeHubVertex(stronghold, 5, 708, 2));
        data.RoomEdits.Add(new TRRoomTextureCreate
        {
            RoomIndex = 5,
            FaceType = TRMeshFaceType.TexturedQuad,
            SourceRoom = 6,
            SourceIndex = 399,
            Vertices = new()
            {
                (ushort)(nextVert + 1),
                nextVert,
                rects[708].Vertices[3],
                rects[708].Vertices[2],
            }
        });

        List<short> faces = new()
        {
            659, 597, 540, 492, 433, 372, 325, 275,
            220, 223, 225, 228, 231, 234, 237, 240, 243,
            298, 347, 405, 468, 521, 570, 632, 685, 733,
            784, 782, 780, 778, 776, 774, 771, 768, 764,
        };
        foreach (short face in faces)
        {
            nextVert++;
            data.RoomEdits.Add(MakeHubVertex(stronghold, 5, face, 2));
            data.RoomEdits.Add(new TRRoomTextureCreate
            {
                RoomIndex = 5,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 6,
                SourceIndex = 399,
                Vertices = new()
                {
                (ushort)(nextVert + 1),
                nextVert,
                rects[face].Vertices[3],
                rects[face].Vertices[2],
                }
            });
        }

        // Pyramid bit
        nextVert += 2;
        ushort start = nextVert;
        data.RoomEdits.Add(MakeHubVertex(stronghold, 5, 607, 3));
        data.RoomEdits.Add(MakeHubVertex(stronghold, 5, 607, 2));
        data.RoomEdits.Add(new TRRoomTextureCreate
        {
            RoomIndex = 5,
            FaceType = TRMeshFaceType.TexturedQuad,
            SourceRoom = GetSource(stronghold, TRMeshFaceType.TexturedQuad, 254).Room,
            SourceIndex = GetSource(stronghold, TRMeshFaceType.TexturedQuad, 254).Face,
            Vertices = new()
            {
                rects[607].Vertices[3],
                rects[607].Vertices[2],
                (ushort)(nextVert + 1),
                nextVert,
            }
        });

        nextVert++;
        data.RoomEdits.Add(MakeHubVertex(stronghold, 5, 668, 2));
        data.RoomEdits.Add(new TRRoomTextureCreate
        {
            RoomIndex = 5,
            FaceType = TRMeshFaceType.TexturedQuad,
            SourceRoom = GetSource(stronghold, TRMeshFaceType.TexturedQuad, 254).Room,
            SourceIndex = GetSource(stronghold, TRMeshFaceType.TexturedQuad, 254).Face,
            Vertices = new()
            {
                rects[668].Vertices[3],
                rects[668].Vertices[2],
                (ushort)(nextVert + 1),
                nextVert,
            }
        });
        nextVert++;
        data.RoomEdits.Add(MakeHubVertex(stronghold, 5, 666, 1));
        data.RoomEdits.Add(new TRRoomTextureCreate
        {
            RoomIndex = 5,
            FaceType = TRMeshFaceType.TexturedQuad,
            SourceRoom = GetSource(stronghold, TRMeshFaceType.TexturedQuad, 225).Room,
            SourceIndex = GetSource(stronghold, TRMeshFaceType.TexturedQuad, 225).Face,
            Vertices = new()
            {
                rects[666].Vertices[0],
                rects[666].Vertices[1],
                (ushort)(nextVert + 1),
                nextVert,
            }
        });

        foreach (short face in new short[] { 669, 672, 675 })
        {
            nextVert++;
            data.RoomEdits.Add(MakeHubVertex(stronghold, 5, face, 0));
            data.RoomEdits.Add(new TRRoomTextureCreate
            {
                RoomIndex = 5,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = GetSource(stronghold, TRMeshFaceType.TexturedQuad, 225).Room,
                SourceIndex = GetSource(stronghold, TRMeshFaceType.TexturedQuad, 225).Face,
                Vertices = new()
                {
                rects[face].Vertices[1],
                rects[face].Vertices[0],
                (ushort)(nextVert + 1),
                nextVert,
                }
            });
        }

        nextVert++;
        data.RoomEdits.Add(MakeHubVertex(stronghold, 5, 677, 0));
        data.RoomEdits.Add(new TRRoomTextureCreate
        {
            RoomIndex = 5,
            FaceType = TRMeshFaceType.TexturedQuad,
            SourceRoom = GetSource(stronghold, TRMeshFaceType.TexturedQuad, 225).Room,
            SourceIndex = GetSource(stronghold, TRMeshFaceType.TexturedQuad, 225).Face,
            Vertices = new()
            {
                rects[677].Vertices[3],
                rects[677].Vertices[0],
                (ushort)(nextVert + 1),
                nextVert,
            }
        });

        nextVert++;
        data.RoomEdits.Add(MakeHubVertex(stronghold, 5, 679, 2));
        data.RoomEdits.Add(new TRRoomTextureCreate
        {
            RoomIndex = 5,
            FaceType = TRMeshFaceType.TexturedQuad,
            SourceRoom = GetSource(stronghold, TRMeshFaceType.TexturedQuad, 254).Room,
            SourceIndex = GetSource(stronghold, TRMeshFaceType.TexturedQuad, 254).Face,
            Vertices = new()
            {
                rects[679].Vertices[3],
                rects[679].Vertices[2],
                (ushort)(nextVert + 1),
                nextVert,
            }
        });

        nextVert++;
        data.RoomEdits.Add(MakeHubVertex(stronghold, 5, 628, 0));
        data.RoomEdits.Add(new TRRoomTextureCreate
        {
            RoomIndex = 5,
            FaceType = TRMeshFaceType.TexturedQuad,
            SourceRoom = GetSource(stronghold, TRMeshFaceType.TexturedQuad, 254).Room,
            SourceIndex = GetSource(stronghold, TRMeshFaceType.TexturedQuad, 254).Face,
            Vertices = new()
            {
                rects[628].Vertices[1],
                rects[628].Vertices[0],
                (ushort)(nextVert + 1),
                nextVert,
            }
        });

        nextVert++;
        data.RoomEdits.Add(MakeHubVertex(stronghold, 5, 626, 3));
        data.RoomEdits.Add(new TRRoomTextureCreate
        {
            RoomIndex = 5,
            FaceType = TRMeshFaceType.TexturedQuad,
            SourceRoom = GetSource(stronghold, TRMeshFaceType.TexturedQuad, 225).Room,
            SourceIndex = GetSource(stronghold, TRMeshFaceType.TexturedQuad, 225).Face,
            Vertices = new()
            {
                rects[626].Vertices[2],
                rects[626].Vertices[3],
                (ushort)(nextVert + 1),
                nextVert,
            }
        });

        foreach (short face in new short[] { 566, 516, 462 })
        {
            nextVert++;
            data.RoomEdits.Add(MakeHubVertex(stronghold, 5, face, 0));
            data.RoomEdits.Add(new TRRoomTextureCreate
            {
                RoomIndex = 5,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = GetSource(stronghold, TRMeshFaceType.TexturedQuad, 225).Room,
                SourceIndex = GetSource(stronghold, TRMeshFaceType.TexturedQuad, 225).Face,
                Vertices = new()
                {
                    rects[face].Vertices[1],
                    rects[face].Vertices[0],
                    (ushort)(nextVert + 1),
                    nextVert,
                }
            });
        }

        nextVert++;
        data.RoomEdits.Add(MakeHubVertex(stronghold, 5, 399, 3));
        data.RoomEdits.Add(new TRRoomTextureCreate
        {
            RoomIndex = 5,
            FaceType = TRMeshFaceType.TexturedQuad,
            SourceRoom = GetSource(stronghold, TRMeshFaceType.TexturedQuad, 225).Room,
            SourceIndex = GetSource(stronghold, TRMeshFaceType.TexturedQuad, 225).Face,
            Vertices = new()
            {
                rects[399].Vertices[2],
                rects[399].Vertices[3],
                (ushort)(nextVert + 1),
                nextVert,
            }
        });

        nextVert++;
        data.RoomEdits.Add(MakeHubVertex(stronghold, 5, 402, 0));
        data.RoomEdits.Add(new TRRoomTextureCreate
        {
            RoomIndex = 5,
            FaceType = TRMeshFaceType.TexturedQuad,
            SourceRoom = GetSource(stronghold, TRMeshFaceType.TexturedQuad, 254).Room,
            SourceIndex = GetSource(stronghold, TRMeshFaceType.TexturedQuad, 254).Face,
            Vertices = new()
            {
                rects[402].Vertices[1],
                rects[402].Vertices[0],
                (ushort)(nextVert + 1),
                nextVert,
            }
        });

        nextVert++;
        data.RoomEdits.Add(MakeHubVertex(stronghold, 5, 397, 2));
        data.RoomEdits.Add(new TRRoomTextureCreate
        {
            RoomIndex = 5,
            FaceType = TRMeshFaceType.TexturedQuad,
            SourceRoom = GetSource(stronghold, TRMeshFaceType.TexturedQuad, 254).Room,
            SourceIndex = GetSource(stronghold, TRMeshFaceType.TexturedQuad, 254).Face,
            Vertices = new()
            {
                rects[397].Vertices[3],
                rects[397].Vertices[2],
                (ushort)(nextVert + 1),
                nextVert,
            }
        });

        nextVert++;
        data.RoomEdits.Add(MakeHubVertex(stronghold, 5, 339, 3));
        data.RoomEdits.Add(new TRRoomTextureCreate
        {
            RoomIndex = 5,
            FaceType = TRMeshFaceType.TexturedQuad,
            SourceRoom = GetSource(stronghold, TRMeshFaceType.TexturedQuad, 225).Room,
            SourceIndex = GetSource(stronghold, TRMeshFaceType.TexturedQuad, 225).Face,
            Vertices = new()
            {
                rects[339].Vertices[2],
                rects[339].Vertices[3],
                (ushort)(nextVert + 1),
                nextVert,
            }
        });

        foreach (short face in new short[] { 337, 335, 333 })
        {
            nextVert++;
            data.RoomEdits.Add(MakeHubVertex(stronghold, 5, face, 0));
            data.RoomEdits.Add(new TRRoomTextureCreate
            {
                RoomIndex = 5,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = GetSource(stronghold, TRMeshFaceType.TexturedQuad, 225).Room,
                SourceIndex = GetSource(stronghold, TRMeshFaceType.TexturedQuad, 225).Face,
                Vertices = new()
                {
                    rects[face].Vertices[1],
                    rects[face].Vertices[0],
                    (ushort)(nextVert + 1),
                    nextVert,
                }
            });
        }

        nextVert++;
        data.RoomEdits.Add(MakeHubVertex(stronghold, 5, 331, 2));
        data.RoomEdits.Add(new TRRoomTextureCreate
        {
            RoomIndex = 5,
            FaceType = TRMeshFaceType.TexturedQuad,
            SourceRoom = GetSource(stronghold, TRMeshFaceType.TexturedQuad, 225).Room,
            SourceIndex = GetSource(stronghold, TRMeshFaceType.TexturedQuad, 225).Face,
            Vertices = new()
            {
                rects[331].Vertices[1],
                rects[331].Vertices[2],
                (ushort)(nextVert + 1),
                nextVert,
            }
        });

        nextVert++;
        data.RoomEdits.Add(MakeHubVertex(stronghold, 5, 383, 0));
        data.RoomEdits.Add(new TRRoomTextureCreate
        {
            RoomIndex = 5,
            FaceType = TRMeshFaceType.TexturedQuad,
            SourceRoom = GetSource(stronghold, TRMeshFaceType.TexturedQuad, 254).Room,
            SourceIndex = GetSource(stronghold, TRMeshFaceType.TexturedQuad, 254).Face,
            Vertices = new()
            {
                rects[383].Vertices[1],
                rects[383].Vertices[0],
                (ushort)(nextVert + 1),
                nextVert,
            }
        });

        nextVert++;
        data.RoomEdits.Add(MakeHubVertex(stronghold, 5, 385, 2));
        data.RoomEdits.Add(new TRRoomTextureCreate
        {
            RoomIndex = 5,
            FaceType = TRMeshFaceType.TexturedQuad,
            SourceRoom = GetSource(stronghold, TRMeshFaceType.TexturedQuad, 254).Room,
            SourceIndex = GetSource(stronghold, TRMeshFaceType.TexturedQuad, 254).Face,
            Vertices = new()
            {
                rects[385].Vertices[3],
                rects[385].Vertices[2],
                (ushort)(nextVert + 1),
                nextVert,
            }
        });

        nextVert++;
        data.RoomEdits.Add(MakeHubVertex(stronghold, 5, 378, 2));
        data.RoomEdits.Add(new TRRoomTextureCreate
        {
            RoomIndex = 5,
            FaceType = TRMeshFaceType.TexturedQuad,
            SourceRoom = GetSource(stronghold, TRMeshFaceType.TexturedQuad, 225).Room,
            SourceIndex = GetSource(stronghold, TRMeshFaceType.TexturedQuad, 225).Face,
            Vertices = new()
            {
                rects[378].Vertices[1],
                rects[378].Vertices[2],
                (ushort)(nextVert + 1),
                nextVert,
            }
        });

        foreach (short face in new short[] { 439, 496, 546 })
        {
            nextVert++;
            data.RoomEdits.Add(MakeHubVertex(stronghold, 5, face, 0));
            data.RoomEdits.Add(new TRRoomTextureCreate
            {
                RoomIndex = 5,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = GetSource(stronghold, TRMeshFaceType.TexturedQuad, 225).Room,
                SourceIndex = GetSource(stronghold, TRMeshFaceType.TexturedQuad, 225).Face,
                Vertices = new()
                {
                    rects[face].Vertices[1],
                    rects[face].Vertices[0],
                    (ushort)(nextVert + 1),
                    nextVert,
                }
            });
        }

        nextVert++;
        data.RoomEdits.Add(new TRRoomTextureCreate
        {
            RoomIndex = 5,
            FaceType = TRMeshFaceType.TexturedQuad,
            SourceRoom = GetSource(stronghold, TRMeshFaceType.TexturedQuad, 225).Room,
            SourceIndex = GetSource(stronghold, TRMeshFaceType.TexturedQuad, 225).Face,
            Vertices = new()
            {
                rects[602].Vertices[1],
                rects[602].Vertices[2],
                start,
                nextVert,
            }
        });

        // Flipmap
        rects = stronghold.Rooms[74].Mesh.Rectangles;
        nextVert = (ushort)stronghold.Rooms[74].Mesh.Vertices.Count;

        data.RoomEdits.Add(MakeHubVertex(stronghold, 74, 217, 3));
        data.RoomEdits.Add(MakeHubVertex(stronghold, 74, 217, 2));
        data.RoomEdits.Add(new TRRoomTextureCreate
        {
            RoomIndex = 74,
            FaceType = TRMeshFaceType.TexturedQuad,
            SourceRoom = 6,
            SourceIndex = 399,
            Vertices = new()
            {
                (ushort)(nextVert + 1),
                nextVert,
                rects[217].Vertices[3],
                rects[217].Vertices[2],
            }
        });

        faces = new()
        {
            220, 222, 225, 228, 231, 234, 237, 240,
            295, 344, 402, 465, 518, 567, 629, 682, 730,
            781, 779, 777, 775, 773, 771, 768, 765, 761,
            705, 656, 594, 537, 489, 430, 369, 322, 272,
        };
        foreach (short face in faces)
        {
            nextVert++;
            data.RoomEdits.Add(MakeHubVertex(stronghold, 74, face, 2));
            data.RoomEdits.Add(new TRRoomTextureCreate
            {
                RoomIndex = 74,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 6,
                SourceIndex = 399,
                Vertices = new()
                {
                    (ushort)(nextVert + 1),
                    nextVert,
                    rects[face].Vertices[3],
                    rects[face].Vertices[2],
                }
            });
        }

        // Pyramid bit
        nextVert += 2;
        start = nextVert;
        data.RoomEdits.Add(MakeHubVertex(stronghold, 74, 604, 3));
        data.RoomEdits.Add(MakeHubVertex(stronghold, 74, 604, 2));
        data.RoomEdits.Add(new TRRoomTextureCreate
        {
            RoomIndex = 74,
            FaceType = TRMeshFaceType.TexturedQuad,
            SourceRoom = GetSource(stronghold, TRMeshFaceType.TexturedQuad, 254).Room,
            SourceIndex = GetSource(stronghold, TRMeshFaceType.TexturedQuad, 254).Face,
            Vertices = new()
            {
                rects[604].Vertices[3],
                rects[604].Vertices[2],
                (ushort)(nextVert + 1),
                nextVert,
            }
        });

        nextVert++;
        data.RoomEdits.Add(MakeHubVertex(stronghold, 74, 665, 2));
        data.RoomEdits.Add(new TRRoomTextureCreate
        {
            RoomIndex = 74,
            FaceType = TRMeshFaceType.TexturedQuad,
            SourceRoom = GetSource(stronghold, TRMeshFaceType.TexturedQuad, 254).Room,
            SourceIndex = GetSource(stronghold, TRMeshFaceType.TexturedQuad, 254).Face,
            Vertices = new()
            {
                rects[665].Vertices[3],
                rects[665].Vertices[2],
                (ushort)(nextVert + 1),
                nextVert,
            }
        });
        nextVert++;
        data.RoomEdits.Add(MakeHubVertex(stronghold, 74, 663, 1));
        data.RoomEdits.Add(new TRRoomTextureCreate
        {
            RoomIndex = 74,
            FaceType = TRMeshFaceType.TexturedQuad,
            SourceRoom = GetSource(stronghold, TRMeshFaceType.TexturedQuad, 225).Room,
            SourceIndex = GetSource(stronghold, TRMeshFaceType.TexturedQuad, 225).Face,
            Vertices = new()
            {
                rects[663].Vertices[0],
                rects[663].Vertices[1],
                (ushort)(nextVert + 1),
                nextVert,
            }
        });

        foreach (short face in new short[] { 666, 669, 672 })
        {
            nextVert++;
            data.RoomEdits.Add(MakeHubVertex(stronghold, 74, face, 0));
            data.RoomEdits.Add(new TRRoomTextureCreate
            {
                RoomIndex = 74,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = GetSource(stronghold, TRMeshFaceType.TexturedQuad, 225).Room,
                SourceIndex = GetSource(stronghold, TRMeshFaceType.TexturedQuad, 225).Face,
                Vertices = new()
                {
                    rects[face].Vertices[1],
                    rects[face].Vertices[0],
                    (ushort)(nextVert + 1),
                    nextVert,
                }
            });
        }

        nextVert++;
        data.RoomEdits.Add(MakeHubVertex(stronghold, 74, 674, 0));
        data.RoomEdits.Add(new TRRoomTextureCreate
        {
            RoomIndex = 74,
            FaceType = TRMeshFaceType.TexturedQuad,
            SourceRoom = GetSource(stronghold, TRMeshFaceType.TexturedQuad, 225).Room,
            SourceIndex = GetSource(stronghold, TRMeshFaceType.TexturedQuad, 225).Face,
            Vertices = new()
            {
                rects[674].Vertices[3],
                rects[674].Vertices[0],
                (ushort)(nextVert + 1),
                nextVert,
            }
        });

        nextVert++;
        data.RoomEdits.Add(MakeHubVertex(stronghold, 74, 676, 2));
        data.RoomEdits.Add(new TRRoomTextureCreate
        {
            RoomIndex = 74,
            FaceType = TRMeshFaceType.TexturedQuad,
            SourceRoom = GetSource(stronghold, TRMeshFaceType.TexturedQuad, 254).Room,
            SourceIndex = GetSource(stronghold, TRMeshFaceType.TexturedQuad, 254).Face,
            Vertices = new()
            {
                rects[676].Vertices[3],
                rects[676].Vertices[2],
                (ushort)(nextVert + 1),
                nextVert,
            }
        });

        nextVert++;
        data.RoomEdits.Add(MakeHubVertex(stronghold, 74, 625, 0));
        data.RoomEdits.Add(new TRRoomTextureCreate
        {
            RoomIndex = 74,
            FaceType = TRMeshFaceType.TexturedQuad,
            SourceRoom = GetSource(stronghold, TRMeshFaceType.TexturedQuad, 254).Room,
            SourceIndex = GetSource(stronghold, TRMeshFaceType.TexturedQuad, 254).Face,
            Vertices = new()
            {
                rects[625].Vertices[1],
                rects[625].Vertices[0],
                (ushort)(nextVert + 1),
                nextVert,
            }
        });

        nextVert++;
        data.RoomEdits.Add(MakeHubVertex(stronghold, 74, 623, 3));
        data.RoomEdits.Add(new TRRoomTextureCreate
        {
            RoomIndex = 74,
            FaceType = TRMeshFaceType.TexturedQuad,
            SourceRoom = GetSource(stronghold, TRMeshFaceType.TexturedQuad, 225).Room,
            SourceIndex = GetSource(stronghold, TRMeshFaceType.TexturedQuad, 225).Face,
            Vertices = new()
            {
                rects[623].Vertices[2],
                rects[623].Vertices[3],
                (ushort)(nextVert + 1),
                nextVert,
            }
        });

        foreach (short face in new short[] { 563, 513, 459 })
        {
            nextVert++;
            data.RoomEdits.Add(MakeHubVertex(stronghold, 74, face, 0));
            data.RoomEdits.Add(new TRRoomTextureCreate
            {
                RoomIndex = 74,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = GetSource(stronghold, TRMeshFaceType.TexturedQuad, 225).Room,
                SourceIndex = GetSource(stronghold, TRMeshFaceType.TexturedQuad, 225).Face,
                Vertices = new()
                {
                    rects[face].Vertices[1],
                    rects[face].Vertices[0],
                    (ushort)(nextVert + 1),
                    nextVert,
                }
            });
        }

        nextVert++;
        data.RoomEdits.Add(MakeHubVertex(stronghold, 74, 396, 3));
        data.RoomEdits.Add(new TRRoomTextureCreate
        {
            RoomIndex = 74,
            FaceType = TRMeshFaceType.TexturedQuad,
            SourceRoom = GetSource(stronghold, TRMeshFaceType.TexturedQuad, 225).Room,
            SourceIndex = GetSource(stronghold, TRMeshFaceType.TexturedQuad, 225).Face,
            Vertices = new()
            {
                rects[396].Vertices[2],
                rects[396].Vertices[3],
                (ushort)(nextVert + 1),
                nextVert,
            }
        });

        nextVert++;
        data.RoomEdits.Add(MakeHubVertex(stronghold, 74, 399, 0));
        data.RoomEdits.Add(new TRRoomTextureCreate
        {
            RoomIndex = 74,
            FaceType = TRMeshFaceType.TexturedQuad,
            SourceRoom = GetSource(stronghold, TRMeshFaceType.TexturedQuad, 254).Room,
            SourceIndex = GetSource(stronghold, TRMeshFaceType.TexturedQuad, 254).Face,
            Vertices = new()
            {
                rects[399].Vertices[1],
                rects[399].Vertices[0],
                (ushort)(nextVert + 1),
                nextVert,
            }
        });

        nextVert++;
        data.RoomEdits.Add(MakeHubVertex(stronghold, 74, 394, 2));
        data.RoomEdits.Add(new TRRoomTextureCreate
        {
            RoomIndex = 74,
            FaceType = TRMeshFaceType.TexturedQuad,
            SourceRoom = GetSource(stronghold, TRMeshFaceType.TexturedQuad, 254).Room,
            SourceIndex = GetSource(stronghold, TRMeshFaceType.TexturedQuad, 254).Face,
            Vertices = new()
            {
                rects[394].Vertices[3],
                rects[394].Vertices[2],
                (ushort)(nextVert + 1),
                nextVert,
            }
        });

        nextVert++;
        data.RoomEdits.Add(MakeHubVertex(stronghold, 74, 336, 3));
        data.RoomEdits.Add(new TRRoomTextureCreate
        {
            RoomIndex = 74,
            FaceType = TRMeshFaceType.TexturedQuad,
            SourceRoom = GetSource(stronghold, TRMeshFaceType.TexturedQuad, 225).Room,
            SourceIndex = GetSource(stronghold, TRMeshFaceType.TexturedQuad, 225).Face,
            Vertices = new()
            {
                rects[336].Vertices[2],
                rects[336].Vertices[3],
                (ushort)(nextVert + 1),
                nextVert,
            }
        });

        foreach (short face in new short[] { 334, 332, 330 })
        {
            nextVert++;
            data.RoomEdits.Add(MakeHubVertex(stronghold, 74, face, 0));
            data.RoomEdits.Add(new TRRoomTextureCreate
            {
                RoomIndex = 74,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = GetSource(stronghold, TRMeshFaceType.TexturedQuad, 225).Room,
                SourceIndex = GetSource(stronghold, TRMeshFaceType.TexturedQuad, 225).Face,
                Vertices = new()
                {
                    rects[face].Vertices[1],
                    rects[face].Vertices[0],
                    (ushort)(nextVert + 1),
                    nextVert,
                }
            });
        }

        nextVert++;
        data.RoomEdits.Add(MakeHubVertex(stronghold, 74, 328, 2));
        data.RoomEdits.Add(new TRRoomTextureCreate
        {
            RoomIndex = 74,
            FaceType = TRMeshFaceType.TexturedQuad,
            SourceRoom = GetSource(stronghold, TRMeshFaceType.TexturedQuad, 225).Room,
            SourceIndex = GetSource(stronghold, TRMeshFaceType.TexturedQuad, 225).Face,
            Vertices = new()
            {
                rects[328].Vertices[1],
                rects[328].Vertices[2],
                (ushort)(nextVert + 1),
                nextVert,
            }
        });

        nextVert++;
        data.RoomEdits.Add(MakeHubVertex(stronghold, 74, 380, 0));
        data.RoomEdits.Add(new TRRoomTextureCreate
        {
            RoomIndex = 74,
            FaceType = TRMeshFaceType.TexturedQuad,
            SourceRoom = GetSource(stronghold, TRMeshFaceType.TexturedQuad, 254).Room,
            SourceIndex = GetSource(stronghold, TRMeshFaceType.TexturedQuad, 254).Face,
            Vertices = new()
            {
                rects[380].Vertices[1],
                rects[380].Vertices[0],
                (ushort)(nextVert + 1),
                nextVert,
            }
        });

        nextVert++;
        data.RoomEdits.Add(MakeHubVertex(stronghold, 74, 382, 2));
        data.RoomEdits.Add(new TRRoomTextureCreate
        {
            RoomIndex = 74,
            FaceType = TRMeshFaceType.TexturedQuad,
            SourceRoom = GetSource(stronghold, TRMeshFaceType.TexturedQuad, 254).Room,
            SourceIndex = GetSource(stronghold, TRMeshFaceType.TexturedQuad, 254).Face,
            Vertices = new()
            {
                rects[382].Vertices[3],
                rects[382].Vertices[2],
                (ushort)(nextVert + 1),
                nextVert,
            }
        });

        nextVert++;
        data.RoomEdits.Add(MakeHubVertex(stronghold, 74, 375, 2));
        data.RoomEdits.Add(new TRRoomTextureCreate
        {
            RoomIndex = 74,
            FaceType = TRMeshFaceType.TexturedQuad,
            SourceRoom = GetSource(stronghold, TRMeshFaceType.TexturedQuad, 225).Room,
            SourceIndex = GetSource(stronghold, TRMeshFaceType.TexturedQuad, 225).Face,
            Vertices = new()
            {
                rects[375].Vertices[1],
                rects[375].Vertices[2],
                (ushort)(nextVert + 1),
                nextVert,
            }
        });

        foreach (short face in new short[] { 436, 493, 543 })
        {
            nextVert++;
            data.RoomEdits.Add(MakeHubVertex(stronghold, 74, face, 0));
            data.RoomEdits.Add(new TRRoomTextureCreate
            {
                RoomIndex = 74,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = GetSource(stronghold, TRMeshFaceType.TexturedQuad, 225).Room,
                SourceIndex = GetSource(stronghold, TRMeshFaceType.TexturedQuad, 225).Face,
                Vertices = new()
                {
                    rects[face].Vertices[1],
                    rects[face].Vertices[0],
                    (ushort)(nextVert + 1),
                    nextVert,
                }
            });
        }

        nextVert++;
        data.RoomEdits.Add(new TRRoomTextureCreate
        {
            RoomIndex = 74,
            FaceType = TRMeshFaceType.TexturedQuad,
            SourceRoom = GetSource(stronghold, TRMeshFaceType.TexturedQuad, 225).Room,
            SourceIndex = GetSource(stronghold, TRMeshFaceType.TexturedQuad, 225).Face,
            Vertices = new()
            {
                rects[599].Vertices[1],
                rects[599].Vertices[2],
                start,
                nextVert,
            }
        });
    }

    private static TRRoomVertexCreate MakeHubVertex(TR1Level stronghold, short room, short rect, short vert)
    {
        TR1RoomVertex copy = stronghold.Rooms[room].Mesh.Vertices[stronghold.Rooms[room].Mesh.Rectangles[rect].Vertices[vert]];
        return new TRRoomVertexCreate
        {
            RoomIndex = room,
            Vertex = new()
            {
                Lighting = 6576,
                Vertex = new()
                {
                    X = copy.Vertex.X,
                    Y = (short)(copy.Vertex.Y + 256),
                    Z = copy.Vertex.Z
                }
            }
        };
    }
}
