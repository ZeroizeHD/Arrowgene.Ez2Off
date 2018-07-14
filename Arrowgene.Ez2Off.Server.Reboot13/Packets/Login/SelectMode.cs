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

using System.Collections.Generic;
using Arrowgene.Ez2Off.Server.Client;
using Arrowgene.Ez2Off.Common.Models;
using Arrowgene.Ez2Off.Server.Models;
using Arrowgene.Ez2Off.Server.Packet;
using Arrowgene.Services.Buffers;

namespace Arrowgene.Ez2Off.Server.Reboot13.Packets.Login
{
    public class SelectMode : Handler<LoginServer>
    {
        public SelectMode(LoginServer server) : base(server)
        {
        }

        public override int Id => 9;

        public override void Handle(EzClient client, EzPacket packet)
        {
            ModeType mode = (ModeType) packet.Data.ReadByte();
            _logger.Debug("Selected Mode: {0}", mode);
            client.Session.Mode = mode;
            IBuffer response = EzServer.Buffer.Provide();
            List<ServerPoint> worldServers = Server.WorldServerPoints;
            response.WriteByte(0);
            response.WriteByte((byte) worldServers.Count);
            foreach (ServerPoint worldServer in worldServers)
            {
                response.WriteByte((byte) worldServer.Id);
                response.WriteInt16(0); // TODO: Implement Server Load
            }

            Send(client, 10, response);
        }
    }
}