import socket
import sys
from time import sleep

sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)

server_address = ('169.254.191.81', 3000)
server_address_2 = ('169.254.191.81', 3001)

try:
    message = '{"timestamp": 1404330711, "angle": 180}'
    print>>sys.stderr, 'sending %s' % message
    sock.sendto(message, server_address)
    sleep(2)
    message = '{"timestamp": 1404330711, "angle": 0}'
    print>>sys.stderr, 'sending %s' % message
    sock.sendto(message, server_address)
    sleep(2)
    message = '{"timestamp": 1404330711, "temperature": 10}'
    print>>sys.stderr, 'sending %s' % message
    sock.sendto(message, server_address_2)
    sleep(2)
    message = '{"timestamp": 1404330711, "temperature": -10}'
    print>>sys.stderr, 'sending %s' % message
    sock.sendto(message, server_address_2)
    sleep(2)
    message = '{"timestamp": 1404330711, "temperature": 0}'
    print>>sys.stderr, 'sending %s' % message
    sock.sendto(message, server_address_2)

finally:
    print >> sys.stderr, 'closing socket'
    sock.close()

