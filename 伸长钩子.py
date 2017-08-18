import pygame, sys, random
import math
from pygame.locals import *



#素材
local_image = 'resources\\images\\'

hook_image = local_image+'hook.png'
images = [local_image + 'gold1.png', local_image + 'gold2.png', local_image + 'gold3.png']
background_image = local_image + 'background.png'
hook_image = local_image+'hook.png'


FPS = 30
MAXRANDTIME = 100
SCORE_POS = 0, 0
INFO_POS = 300, 200
RED = 255, 0, 0

start_x, start_y = 400, 100

# 游戏状态常量定义
GAME_STATE_INIT = 0
GAME_STATE_RUN  = 1
GAME_STATE_PAUSE = 2
GAME_STATE_SUCCESS = 3
GAME_STATE_OVER = 4

#
FAIL_COUNT = 3
WIN_COUNT = GOLD_COUNT = 10


class Gold(pygame.sprite.Sprite):
	def __init__(self, position):
		pygame.sprite.Sprite.__init__(self)
		#随机产生一个金块
		self.size = random.randint(0, 2)
		self.image = pygame.image.load(images[self.size])
		self.rect = self.image.get_rect()
		self.rect.center = position

class Direction():

	def __init__(self):
		self.length = 30#绳子的最长长度

		self.angle = 0#角度  0度是水平往右 180 是水平往左
		self.left = True#往左还是往右摇摆
		self.x,self.y = 0,0#坐标
		self.pause = False#是否暂停摇摆

	def move(self):
		#print(self.angle)
		if self.left == True:
			self.angle += 3  # <--
		else:
			self.angle -= 3
		angle = self.angle/180*math.pi
		#计算出
		self.x = start_x + self.length * math.cos(angle)
		self.y = start_y + self.length * math.sin(angle)
		self.switch()#判断要不 要改变摇摆方向

	def switch(self):#如果往左摆而且钩子摆动到180左右
		if 180<=self.angle  and self.left == True:
			self.left= False
			#如果往右摆而且钩子摆动到0 左右
		if self.angle <= 0 and self.left == False:
			self.left = True

class Hook():  # 钩子
	def __init__(self):
		self.image = pygame.image.load(hook_image)
		
		self.length = 500  # 最长长度
		self.direction = None
		self.ready()

	# 准备下一次放钩子
	def ready(self):
		self.x, self.y = start_x, start_y  # 起点
		self.pause, self.down = True, False
		#self.is_carrying = False
		#self.carry = None
		self.speed = 7  # 速度
		if self.direction != None:
			self.direction.pause = False
	#
	def mine(self):
		'''
		if self.is_carrying == True:
			if self.carry.size == 0:
				self.speed = 10
			elif self.carry.size == 1:
				self.speed = 6
			elif self.carry.size == 2:
				self.speed = 2
				'''
		if not self.pause:
			self.direction.pause = True
			if self.down == False:
				self.go('up')
			elif self.down:
				self.go('down')
		self.switch()

	def go(self, direction):  # 计算速度的分量 然后计算当前钩子位置
		move_x, move_y = self.get_v(self.speed)
		if direction == 'up':
			move_x, move_y = -move_x, -move_y
		self.x += move_x
		self.y += move_y

	def get_distance(self, a, b):
		dis = math.sqrt((a[0] - b[0]) ** 2 + (a[1] - b[1]) ** 2)
		return dis

	def switch(self):  # 切换钩子回收 和 放下
		if self.down and self.get_distance([self.x, self.y],
										   [start_x, start_y]) > self.length:  # 如果放下 钩子的时候 距离超过最大长度

			self.down = False
		if self.y - start_y < 0:

			self.ready()  # 准备下一次钩子

	def get_v(self, speed):
		angle = self.direction.angle
		a, b = math.cos(angle / 180 * math.pi), math.sin(angle / 180 * math.pi)
		return speed * a, speed * b


	def hook_position(self):
		# 钩子中心点固定
		if not self.pause:
			center = (int(self.x), int(self.y))
		else:
			center = (int(self.direction.x), int(self.direction.y))
		return center

# 打气球游戏类
class GoldMiner():
	def __init__(self):
		# 初始化游戏界面
		pygame.init()
		self.game_font = pygame.font.SysFont('simhei', 24)
		self.mainClock = pygame.time.Clock()
		# 加载图片
		self.background_image = pygame.image.load(background_image)
		self.hook = Hook()
		self.hook.direction = Direction()# self.direction
		self.windowSurface = pygame.display.set_mode((800, 600))
		pygame.display.set_caption('黄金矿工')
		self.background_image = pygame.transform.scale(self.background_image, (800, 600))
		self.windowSurface.blit(self.background_image, (0, 0))


		self.game_state = GAME_STATE_INIT


	# 打印文字
	def print_text(self, pos, text, color=(0, 0, 0)):
		imgText = self.game_font.render(text, True, color)
		self.windowSurface.blit(imgText, pos)



	def run(self):

		while True:
			for event in pygame.event.get():
				if event.type == QUIT:
					sys.exit()
				if event.type == KEYDOWN:

					if event.key == K_DOWN:
						if self.hook.pause == True:
							self.hook.pause = False
							self.hook.down = True
			# 游戏控制流程
			# 初始时
			if self.game_state == GAME_STATE_INIT:
				self.game_state = GAME_STATE_RUN
			# 运行时
			elif self.game_state == GAME_STATE_RUN:
				if self.hook.direction.pause == False:
					self.hook.direction.move()
				# 开始运行钩子
				self.hook.mine()
				self.windowSurface.blit(self.background_image, (0, 0))
				# 绘制表示角度的圆点
				#pygame.draw.circle(self.windowSurface, RED, (int(self.direction.x), int(self.direction.y)), 3, 0)

				# 绘制钩子
				hook_image_rotate = pygame.transform.rotate(self.hook.image, -self.hook.direction.angle)
				hook_rect = hook_image_rotate.get_rect()
				hook_rect.center = self.hook.hook_position()
				self.windowSurface.blit(hook_image_rotate, hook_rect)
				# 绘制钩子的线
				pygame.draw.line(self.windowSurface, (0, 0, 0), (start_x, start_y),
								 (int(self.hook.x), int(self.hook.y)))

			# 结束时
			elif self.game_state == GAME_STATE_OVER:
				# 绘制文字描述信息
				text = "游戏结束"
				self.print_text(INFO_POS, text)
			# 胜利时
			elif self.game_state == GAME_STATE_SUCCESS:
				# 绘制文字描述信息
				text = "游戏胜利"
				self.print_text(INFO_POS, text)

			pygame.display.update()
			self.mainClock.tick(FPS)

#运行游戏
app = GoldMiner()
app.run()

# 游戏主循环