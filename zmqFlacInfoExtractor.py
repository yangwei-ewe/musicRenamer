# -*- coding: utf-8 -*-
import zmq

ctx=zmq.Context()
socket = ctx.socket(zmq.REP)
socket.bind("tcp://127.0.0.1:5556")
datastr= socket.recv_string()

if datastr[0] == "0":
    plusZero = "0000"
else:
    plusZero = ""
binstr= plusZero + bin(int(datastr, 16))[2:]
hzRate = int(binstr[:20], 2)
channelsNum = int(binstr[20:23], 2) + 1
bps = int(binstr[23:28], 2) + 1

socket.send_string(str(hzRate)+","+str(channelsNum)+","+str(bps))
