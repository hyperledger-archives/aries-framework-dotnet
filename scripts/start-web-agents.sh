#!/usr/bin/env bash

echo 'Starting NGROK'
ngrok start -config web-agents-ngrok-config.yaml --all > /dev/null &
sleep 2s
ngrok_hosts=$(curl --silent --show-error http://localhost:5037/api/tunnels | sed -nE 's/.*public_url":"http:..([^"]*).*public_url":"http:..([^"]*).*/\1|\2/p')
IFS="|" read -r host2 host1 <<< "$ngrok_hosts"

export HOST1="http://$host1"
export HOST2="http://$host2"

echo "NGROK started"

echo "Starting Web Agents with public urls $HOST1 $HOST2"
docker-compose -f ../docker-compose.yaml build
docker-compose -f ../docker-compose.yaml up