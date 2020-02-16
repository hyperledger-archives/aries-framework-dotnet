########################################################
##  Creates a pool of nodes that uses 
##  the payment plugins with plenum.
##
##	run the following commands to build and start
##	the pool.
##	
##	localhost
##  	docker build -f indy-pool.dockerfile -t indy_pool .
##		docker run -itd -p 9701-9708:9701-9708 indy_pool
##	
##	specific IP address
##		# replace 192.168.179.90 with your wifi IP address
##		docker build --build-arg pool_ip=192.168.179.90 -f ci/indy-pool.dockerfile -t indy_pool .
##		docker run -itd -p 192.168.179.90:9701-9708:9701-9708 indy_pool
##  
##	docker network
##		docker network create --subnet 10.0.0.0/8 indy_pool_network
##		docker build --build-arg pool_ip=10.0.0.2 -f ci/indy-pool.dockerfile -t indy_pool .
##		docker run -d --ip="10.0.0.2" --net=indy_pool_network indy_pool
##	
##	'docker ps' will show the container id
##
##  To connect to the shell of the container you can use 
##	the following exec command replacing the <container_id> 
##  with the id of the running container.
##
##  docker exec -it <container_id> /bin/bash
##
########################################################FROM ubuntu:16.04
FROM ubuntu:16.04

ARG uid=1000

# Install environment
RUN apt-get update -y && apt-get install -y \
	git \
	wget \
	python3.5 \
	python3-pip \
	python-setuptools \
	python3-nacl \
	apt-transport-https \
	ca-certificates \
	supervisor

RUN pip3 install -U \
	pip==9.0.3 \
	setuptools

RUN apt-key adv --keyserver keyserver.ubuntu.com --recv-keys CE7709D068DB5E88
ARG indy_stream=stable
RUN echo "deb https://repo.sovrin.org/deb xenial $indy_stream" >> /etc/apt/sources.list

RUN useradd -ms /bin/bash -u $uid indy

ARG indy_plenum_ver=1.12.0
ARG indy_node_ver=1.12.0
ARG python3_indy_crypto_ver=0.4.5
ARG indy_crypto_ver=0.4.5
ARG python3_pyzmq_ver=18.1.0
ARG sovtoken_ver=1.0.5
ARG sovtokenfees_ver=1.0.5

RUN apt-get update -y && apt-get install -y \
	python3-pyzmq=${python3_pyzmq_ver} \
	indy-plenum=${indy_plenum_ver} \
	indy-node=${indy_node_ver} \
	python3-indy-crypto=${python3_indy_crypto_ver} \
	libindy-crypto=${indy_crypto_ver} \
	sovtoken=${sovtoken_ver} \
	sovtokenfees=${sovtokenfees_ver} \
    vim

RUN echo "[supervisord]\n\
logfile = /tmp/supervisord.log\n\
logfile_maxbytes = 50MB\n\
logfile_backups=10\n\
logLevel = error\n\
pidfile = /tmp/supervisord.pid\n\
nodaemon = true\n\
minfds = 1024\n\
minprocs = 200\n\
umask = 022\n\
user = indy\n\
identifier = supervisor\n\
directory = /tmp\n\
nocleanup = true\n\
childlogdir = /tmp\n\
strip_ansi = false\n\
\n\
[program:node1]\n\
command=start_indy_node Node1 0.0.0.0 9701 0.0.0.0 9702\n\
directory=/home/indy\n\
stdout_logfile=/tmp/node1.log\n\
stderr_logfile=/tmp/node1.log\n\
\n\
[program:node2]\n\
command=start_indy_node Node2 0.0.0.0 9703 0.0.0.0 9704\n\
directory=/home/indy\n\
stdout_logfile=/tmp/node2.log\n\
stderr_logfile=/tmp/node2.log\n\
\n\
[program:node3]\n\
command=start_indy_node Node3 0.0.0.0 9705 0.0.0.0 9706\n\
directory=/home/indy\n\
stdout_logfile=/tmp/node3.log\n\
stderr_logfile=/tmp/node3.log\n\
\n\
[program:node4]\n\
command=start_indy_node Node4 0.0.0.0 9707 0.0.0.0 9708\n\
directory=/home/indy\n\
stdout_logfile=/tmp/node4.log\n\
stderr_logfile=/tmp/node4.log\n"\
>> /etc/supervisord.conf

USER indy

RUN awk '{if (index($1, "NETWORK_NAME") != 0) {print("NETWORK_NAME = \"sandbox\"")} else print($0)}' /etc/indy/indy_config.py> /tmp/indy_config.py
RUN mv /tmp/indy_config.py /etc/indy/indy_config.py

ARG pool_ip=127.0.0.1

RUN generate_indy_pool_transactions --nodes 4 --clients 5 --nodeNum 1 2 3 4 --ips="$pool_ip,$pool_ip,$pool_ip,$pool_ip"

EXPOSE 9701 9702 9703 9704 9705 9706 9707 9708

CMD ["/usr/bin/supervisord"]
