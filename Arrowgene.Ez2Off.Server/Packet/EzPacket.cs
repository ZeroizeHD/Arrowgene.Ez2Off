﻿/*
 * This file is part of Arrowgene.Ez2Off
 *
 * Arrowgene.Ez2Off is a server implementation for the game "Ez2On".
 * Copyright (C) 2017-2018 Sebastian Heinz
 * Copyright (C) 2017-2018 Halgulaea
 * Copyright (C) 2017-2018 David Via
 *
 * Github: https://github.com/Arrowgene/Arrowgene.Ez2Off
 *
 * Arrowgene.Ez2Off is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * Arrowgene.Ez2Off is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with Arrowgene.Ez2Off. If not, see <https://www.gnu.org/licenses/>.
 */

using Arrowgene.Services.Buffers;

namespace Arrowgene.Ez2Off.Server.Packet
{
    public class EzPacket
    {
        public const int HeaderSize = 7;

        public EzPacket(byte id, IBuffer buffer)
        {
            Data = buffer;
            Id = id;
        }

        public IBuffer ToData()
        {
            IBuffer data = EzServer.Buffer.Provide();
            data.WriteByte(Id);
            data.WriteInt16((short) Data.Size, Endianness.Big);
            data.WriteInt32(0);
            data.WriteBuffer(Data);
            return data;
        }

        public IBuffer Data { get; }
        public byte Id { get; }
    }
}