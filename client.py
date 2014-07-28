import socket
import sys
from time import sleep

sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)

servo_server = ('169.254.191.81', 3000)
peltier_server = ('169.254.191.81', 3001)

try:
    message = '{"timestamp": 1404330711, "angle": 180}'
    print>>sys.stderr, 'sending %s' % message
    sock.sendto(message, servo_server)
    sleep(5)
    message = '{"timestamp": 1404330711, "angle": 5}'
    print>>sys.stderr, 'sending %s' % message
    sock.sendto(message, servo_server)
    sleep(5)
    message = '{"timestamp": 1404330711, "angle": 0}'
    print>>sys.stderr, 'sending %s' % message
    sock.sendto(message, servo_server)
    sleep(5)
    message = '{"timestamp": 1404330711, "temperature": 10}'
    print>>sys.stderr, 'sending %s' % message
    sock.sendto(message, peltier_server)
    sleep(5)
    message = '{"timestamp": 1404330711, "temperature": -10}'
    print>>sys.stderr, 'sending %s' % message
    sock.sendto(message, peltier_server)
    sleep(5)
    message = '{"timestamp": 1404330711, "temperature": 0}'
    print>>sys.stderr, 'sending %s' % message
    sock.sendto(message, peltier_server)

finally:
    print >> sys.stderr, 'closing socket'
    sock.close()

