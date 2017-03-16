import gym
import time
import universe # register the universe environments
import numpy as np
import numpy as np
from matplotlib import pyplot as plt
import math
from skimage import data
from skimage.feature import corner_harris, corner_subpix, corner_peaks
from skimage.transform import warp, AffineTransform
from skimage.draw import ellipse

from RL_brain import DeepQNetwork
cur_y,cur_x = 0,0
def isValidObservation(s):
    return s is not None and len(s) > 0 and s[0] is not None and type(s[0]) == dict and 'vision' in s[0]

def intToVNCAction(a, x, y):
    small_step = 5
    minY = 125
    maxY = 125+160
    minX = 10
    maxX = 10+160
    if a == 0:
        return [universe.spaces.PointerEvent(x, y, 0),
            universe.spaces.PointerEvent(x, y, 1),
            universe.spaces.PointerEvent(x, y, 0)], x, y
    elif a == 1:
        return [universe.spaces.PointerEvent(x, y, 0)], x, y
    elif a == 2:
        if y + small_step <= maxY:
            return [universe.spaces.PointerEvent(x, y + small_step, 0)], x, y + small_step
    elif a == 3:
        if x + small_step <= maxX:
            return [universe.spaces.PointerEvent(x + small_step, y, 0)], x + small_step, y
    elif a == 4:
        if y - small_step >= minY:
            return [universe.spaces.PointerEvent(x, y - small_step, 0)], x, y - small_step
    elif a == 5:
        if x - small_step >= minX:
            return [universe.spaces.PointerEvent(x - small_step, y, 0)], x - small_step, y
    return [], x, y
def rgb2gray(rgb):
  r, g, b = rgb[:,:,0], rgb[:,:,1], rgb[:,:,2]
  gray = 0.2989 * r + 0.5870 * g + 0.1140 * b

  return gray
def centre_button(ob):

  if ob is None: return -1,-1
  x = ob['vision']
  crop = x[75:75+50+160, 10:10+160, :]               # miniwob coordinates crop
  square = x[75+50:75+50+160, 10:10+160, :]  
  gray =rgb2gray(square)
  coords = corner_peaks(corner_harris(gray), min_distance=5)
  coords_subpix = corner_subpix(gray, coords, window_size=13)
  newy = coords_subpix[:,0]
  newx = coords_subpix[:,1]
  newy = newy[np.logical_not(np.isnan(newy))]
  newx = newx[np.logical_not(np.isnan(newx))]

  goal_y,goal_x = np.mean(newy)+125,np.mean(newx)+10
  if math.isnan(goal_y) or math.isnan(goal_x):
    return -1,-1
  
  return goal_y,goal_x




env = gym.make('wob.mini.ClickTest-v0')
# automatically creates a local docker container
env.configure(remotes=1, fps=5,
              vnc_driver='go', 
              vnc_kwargs={'encoding': 'tight', 'compress_level': 0, 
                          'fine_quality_level': 100, 'subsample_level': 0})
observation_n = env.reset()
n_act =5
n_features =2
cur_y,cur_x=0,0
RL = DeepQNetwork(n_act, n_features,
              learning_rate=0.01,
              reward_decay=0.9,
              e_greedy=0.9,
              hidden_layers=[10, 10],
              replace_target_iter=200,
              memory_size=2000,
              # output_graph=True
              )
while True:


            # RL choose action based on observation
  #print observation_n
  #action = RL.choose_action(observation)
  if not isValidObservation(observation_n):
    time.sleep(1000)
    continue
  click_y,click_x = centre_button(observation_n[0])
  if click_y ==-1:
    continue

  state=[click_y-cur_y,click_x-cur_x]
  state = np.array(state)
  action_index = RL.choose_action(state)
  operation,x,y = intToVNCAction(action_index, cur_x, cur_y)

  
  observation_n, reward_n, done_n, info = env.step(operation)

  cur_y,cur_x= y,x;

  click_y,click_x = centre_button(observation_n[0])
  next_state=[click_y-cur_y,click_x-cur_x]
  next_state = np.array(next_state)
  RL.store_transition(state, action_index, reward_n[0], next_state)

  print state,next_state
  if (step > 200) and (step % 5 == 0):
      RL.learn()
  env.render()
