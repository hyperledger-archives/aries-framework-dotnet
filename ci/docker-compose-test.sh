#!/bin/bash
#
# Copyright Hyperledger Aries contributors. All rights reserved.
#
# SPDX-License-Identifier: Apache-2.0

set -ex

function cleanup(){
  if [ "$(docker ps -a -q)" != "" ] ; then
    docker stop $(docker ps -a -q)
    docker rm $(docker ps -a -q)
  fi
  docker network prune -f
}

cleanup
docker-compose -f docker-compose.test.yaml up --build --abort-on-container-exit --exit-code-from test-agent
cleanup
